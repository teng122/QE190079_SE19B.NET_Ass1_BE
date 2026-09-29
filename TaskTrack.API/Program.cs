using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────
var databaseUrl = builder.Configuration["DATABASE_URL"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Set DATABASE_URL or ConnectionStrings:DefaultConnection in configuration.");
var connectionString = databaseUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
    || databaseUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
    ? ParseDatabaseUrl(databaseUrl)
    : new NpgsqlConnectionStringBuilder(databaseUrl).ConnectionString;

builder.Services.AddDbContext<TaskManagementContext>(options =>
    options.UseNpgsql(connectionString));

// ── Repository DI ─────────────────────────────────────────
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// ── Service DI ────────────────────────────────────────────
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITagService, TagService>();

// ── Controllers & Swagger ─────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TaskTrack API", Version = "v1" });
});

// ── CORS ──────────────────────────────────────────────────
var frontendOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
frontendOrigins = frontendOrigins
    .Concat((builder.Configuration["FRONTEND_URL"] ?? string.Empty)
        .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
    .Where(origin => Uri.TryCreate(origin, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

if (builder.Environment.IsDevelopment())
{
    frontendOrigins = frontendOrigins
        .Concat(["http://localhost:3000", "http://localhost:5173"])
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins(frontendOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// ── Pipeline ──────────────────────────────────────────────
var swaggerEnabled = app.Environment.IsDevelopment()
    || bool.TryParse(app.Configuration["SWAGGER_ENABLED"], out var swaggerEnvironmentOverride)
        && swaggerEnvironmentOverride
    || bool.TryParse(app.Configuration["Swagger:Enabled"], out var swaggerConfigurationOverride)
        && swaggerConfigurationOverride;

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskTrack API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("Frontend");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

static string ParseDatabaseUrl(string value)
{
    if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)
        || (uri.Scheme != "postgres" && uri.Scheme != "postgresql"))
    {
        return new NpgsqlConnectionStringBuilder(value).ConnectionString;
    }

    var userInfo = uri.UserInfo.Split(':', 2);
    var builder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.IsDefaultPort ? 5432 : uri.Port,
        Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
        Username = Uri.UnescapeDataString(userInfo[0]),
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty
    };

    foreach (var parameter in uri.Query.TrimStart('?')
        .Split('&', StringSplitOptions.RemoveEmptyEntries))
    {
        var parts = parameter.Split('=', 2);
        var name = Uri.UnescapeDataString(parts[0]).ToLowerInvariant();
        var parameterValue = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;
        if (name == "sslmode" && Enum.TryParse<Npgsql.SslMode>(parameterValue, true, out var sslMode))
        {
            builder.SslMode = sslMode;
        }
    }

    return builder.ConnectionString;
}

# TaskTrack backend

ASP.NET Core 8 Web API for the Task Management assignment. The API uses a three-layer structure:

- `TaskTrack.API` — HTTP controllers and application startup
- `TaskTrack.Service` — validation, application services, and DTOs
- `TaskTrack.Repo` — EF Core PostgreSQL context, entities, and repositories

All endpoints are public and are rooted at `/api`. Swagger is available at `/swagger` in Development. For the assignment's deployed API documentation, set `SWAGGER_ENABLED=true` on the Render service.

## Local setup

1. Create the PostgreSQL database and run the supplied `TaskManagementDB_Postgres.sql` script without changing its schema.
2. Copy `.env.example` values into your local environment and set `DATABASE_URL` to either a PostgreSQL URI or an Npgsql connection string. The local `ConnectionStrings:DefaultConnection` in `TaskTrack.API/appsettings.json` is a password-free fallback.
3. From this directory, restore, build, and run:

   ```powershell
   dotnet restore QE190079_SE19B.NET_Ass1_BE.sln
   dotnet build QE190079_SE19B.NET_Ass1_BE.sln
   dotnet run --project TaskTrack.API\TaskTrack.API.csproj
   ```

The local API listens at `http://localhost:5195`; set the frontend's `NEXT_PUBLIC_API_URL` to that origin. The `TaskTrack.API/TaskTrack.API.http` file contains example requests for the API.

## Render configuration

Create a Web Service from the backend repository and configure:

- Runtime: **Docker**
- Root Directory: leave blank
- Dockerfile Path: `./Dockerfile`

- `DATABASE_URL` — the Render PostgreSQL connection string
- `FRONTEND_URL` — the deployed Vercel origin (no trailing slash)
- `ASPNETCORE_ENVIRONMENT=Production`
- `SWAGGER_ENABLED=true` — enables Swagger for the assignment demo

Render builds and starts the container from the repository's `Dockerfile`; do not enter separate build or start commands. The image listens on port `10000`. Use the PostgreSQL **Internal Database URL** when the web service and database are in the same Render region. Run the supplied SQL initialization script against the Render database before opening the frontend.

Do not commit database credentials; use the hosting provider's environment-variable settings.

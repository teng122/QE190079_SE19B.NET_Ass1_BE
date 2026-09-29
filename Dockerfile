FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY QE190079_SE19B.NET_Ass1_BE.sln ./
COPY TaskTrack.API/TaskTrack.API.csproj TaskTrack.API/
COPY TaskTrack.Service/TaskTrack.Service.csproj TaskTrack.Service/
COPY TaskTrack.Repo/TaskTrack.Repo.csproj TaskTrack.Repo/

RUN dotnet restore QE190079_SE19B.NET_Ass1_BE.sln

COPY . .
RUN dotnet publish TaskTrack.API/TaskTrack.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskTrack.API.dll"]

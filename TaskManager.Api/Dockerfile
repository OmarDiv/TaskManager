# Use the official .NET 9 SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["TaskManager.Domain/TaskManager.Domain.csproj", "TaskManager.Domain/"]
COPY ["TaskManager.Application/TaskManager.Application.csproj", "TaskManager.Application/"]
COPY ["TaskManager.Infrastructure/TaskManager.Infrastructure.csproj", "TaskManager.Infrastructure/"]
COPY ["TaskManager.Api/TaskManager.Api.csproj", "TaskManager.Api/"]

RUN dotnet restore "TaskManager.Api/TaskManager.Api.csproj"

# Copy the remaining files and build the application
COPY . .
WORKDIR "/src/TaskManager.Api"
RUN dotnet build "TaskManager.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TaskManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 9 runtime image as a final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]

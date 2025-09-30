# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["AuthSystem.sln", "./"]
COPY ["src/AuthSystem.API/AuthSystem.API.csproj", "src/AuthSystem.API/"]
COPY ["src/AuthSystem.Application/AuthSystem.Application.csproj", "src/AuthSystem.Application/"]
COPY ["src/AuthSystem.Domain/AuthSystem.Domain.csproj", "src/AuthSystem.Domain/"]
COPY ["src/AuthSystem.Infrastructure/AuthSystem.Infrastructure.csproj", "src/AuthSystem.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "AuthSystem.sln"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/AuthSystem.API"
RUN dotnet build "AuthSystem.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "AuthSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy published files
COPY --from=publish /app/publish .

# Install EF Core tools for migrations
RUN dotnet tool install --global dotnet-ef --version 8.0.0
ENV PATH="${PATH}:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "AuthSystem.API.dll"]

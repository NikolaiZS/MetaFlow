# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all project files
COPY MetaFlow.Domain/MetaFlow.Domain.csproj MetaFlow.Domain/
COPY MetaFlow.Infrastructure/MetaFlow.Infrastructure.csproj MetaFlow.Infrastructure/
COPY MetaFlow.Contracts/MetaFlow.Contracts.csproj MetaFlow.Contracts/
COPY MetaFlow.Api/MetaFlow.Api.csproj MetaFlow.Api/

# Restore dependencies
RUN dotnet restore MetaFlow.Api/MetaFlow.Api.csproj

# Copy everything else
COPY . ./

# Publish
RUN dotnet publish MetaFlow.Api/MetaFlow.Api.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser -d /app -s /sbin/nologin appuser

# Copy published app
COPY --from=build /app/publish ./

# Change ownership
RUN chown -R appuser:appuser /app

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Switch to non-root user
USER appuser

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the app
ENTRYPOINT ["dotnet", "MetaFlow.Api.dll"]

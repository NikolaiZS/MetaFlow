using Carter;

namespace MetaFlow.Api.Features.HealthCheck;

public class HealthCheckModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", () => Results.Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "MetaFlow API",
            version = "1.0.0"
        }))
        .WithName("Health")
        .WithTags("Health");
    }
}

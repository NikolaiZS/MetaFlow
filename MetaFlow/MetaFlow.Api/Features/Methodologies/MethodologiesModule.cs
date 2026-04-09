using Carter;
using MediatR;
using MetaFlow.Api.Features.Methodologies.GetMethodologies;
using MetaFlow.Contracts.Methodologies;

namespace MetaFlow.Api.Features.Methodologies
{
    public class MethodologiesModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/methodologies")
                .WithTags("Methodologies");

            group.MapGet("", async (ISender sender) =>
            {
                var query = new GetMethodologiesQuery();
                var result = await sender.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("GetMethodologies")
            .Produces<List<MethodologyResponse>>(StatusCodes.Status200OK);
        }
    }
}
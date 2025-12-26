using Carter;
using FluentValidation;
using MetaFlow.Api.Common.Behaviors;
using MetaFlow.Api.Common.Exceptions;
using MetaFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MetaFlow.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCarter();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}

using FluentValidation;
using MediatR;
using MetaFlow.Api.Common.Abstractions;
using System.Reflection;

namespace MetaFlow.Api.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
            return CreateFailureResult(errorMessage);
        }

        return await next();
    }

    private static TResponse CreateFailureResult(string errorMessage)
    {
        var responseType = typeof(TResponse);

        if (!responseType.IsGenericType ||
            responseType.GetGenericTypeDefinition() != typeof(Result<>))
        {
            throw new InvalidOperationException("TResponse must be Result<T>");
        }

        var valueType = responseType.GetGenericArguments()[0];

        // Ищем generic метод Failure<T> в базовом классе Result
        var failureMethod = typeof(Result)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "Failure" &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType == typeof(string));

        if (failureMethod == null)
        {
            throw new InvalidOperationException("Could not find Result.Failure<T>(string) method");
        }

        // Создаем конкретную версию метода для нашего типа
        var genericMethod = failureMethod.MakeGenericMethod(valueType);

        // Вызываем метод
        var result = genericMethod.Invoke(null, new object[] { errorMessage });

        if (result == null)
        {
            throw new InvalidOperationException("Failure method returned null");
        }

        return (TResponse)result;
    }
}

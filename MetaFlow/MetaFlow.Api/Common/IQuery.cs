using MediatR;
using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Common
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
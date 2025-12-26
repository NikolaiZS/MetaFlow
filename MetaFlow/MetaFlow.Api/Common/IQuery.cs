using MediatR;

namespace MetaFlow.Api.Common
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }

}

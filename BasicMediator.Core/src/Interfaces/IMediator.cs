using System.Threading;
using System.Threading.Tasks;

namespace BasicMediator.Core.src.Interfaces
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        Task Publish<TNotification>(INotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification;
    }
}
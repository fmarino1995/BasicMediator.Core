using BasicMediator.Core.src.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BasicMediator.Core.src.Implementations
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _provider;

        public Mediator(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handler = _provider.GetService(handlerType);
            if (handler == null)
                throw new InvalidOperationException($"Handler not found for {request.GetType().Name}");

            return await (Task<TResponse>)handlerType
                .GetMethod("Handle")!
                .Invoke(handler, new object[] { request, cancellationToken })!;
        }

        public async Task Publish<TNotification>(INotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
            var handlers = _provider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                await (Task)handlerType
                    .GetMethod("Handle")!
                    .Invoke(handler, new object[] { notification, cancellationToken })!;
            }
        }
    }
}
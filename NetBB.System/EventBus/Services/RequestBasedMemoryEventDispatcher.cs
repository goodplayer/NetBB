using Microsoft.Extensions.DependencyInjection;
using NetBB.System.EventBus.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Services
{
    public class RequestBasedMemoryEventDispatcher : IEventDispatcher, IEventDiaptcherInternal
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IAggregateCrowd aggregateCrowd;

        private readonly ConcurrentQueue<Func<Task>> queue = new();

        public RequestBasedMemoryEventDispatcher(IServiceScopeFactory serviceScopeFactory, IAggregateCrowd aggregateCrowd)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.aggregateCrowd = aggregateCrowd;
        }

        public Task Apply<T>(T @event) where T : Event
        {
            var eventType = @event.GetType();
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            using var scoped = serviceScopeFactory.CreateAsyncScope();
            var services = scoped.ServiceProvider.GetServices(handlerType);
            if (services.Any())
            {
                IFollowUpDispatcher followUpDispatcher = new OnceTimeMemoryFollowUpDispatcher(this.serviceScopeFactory, aggregateCrowd, this);
                DomainContainer domainContainer = new(this.aggregateCrowd, this, followUpDispatcher);
                foreach (var service in services)
                {
                    var srv = (IEventHandler<T>)service;
                    Func<Task> task = async () =>
                    {
                        await srv.HandleEvent(@event, domainContainer);
                    };
                    queue.Enqueue(task);
                }
            }

            return Task.CompletedTask;
        }

        public async Task RunEventHandling()
        {
            while (!queue.IsEmpty)
            {
                Func<Task> task = null;
                if (queue.TryDequeue(out task))
                {
                    await task.Invoke();
                }
            }
        }
    }
}

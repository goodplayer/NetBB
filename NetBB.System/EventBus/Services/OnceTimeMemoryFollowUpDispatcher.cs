using Microsoft.Extensions.DependencyInjection;
using NetBB.System.EventBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Services
{
    public class OnceTimeMemoryFollowUpDispatcher : IFollowUpDispatcher
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IAggregateCrowd aggregateCrowd;
        private readonly IEventDispatcher eventDispatcher;

        public OnceTimeMemoryFollowUpDispatcher(IServiceScopeFactory serviceScopeFactory, IAggregateCrowd aggregateCrowd, IEventDispatcher eventDispatcher)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.aggregateCrowd = aggregateCrowd;
            this.eventDispatcher = eventDispatcher;
        }

        public async Task Apply<T>(T @event) where T : Event
        {
            var eventType = @event.GetType();
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            using var scoped = this.serviceScopeFactory.CreateAsyncScope();
            var services = scoped.ServiceProvider.GetServices(handlerType);
            if (services.Any())
            {
                IFollowUpDispatcher followUpDispatcher = new OnceTimeMemoryFollowUpDispatcher(this.serviceScopeFactory, aggregateCrowd, eventDispatcher);
                DomainContainer domainContainer = new(this.aggregateCrowd, this.eventDispatcher, followUpDispatcher);
                foreach (var service in services)
                {
                    var srv = (IEventHandler<T>)service;
                    await srv.HandleEvent(@event, domainContainer);
                }
            }
        }
    }
}

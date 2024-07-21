using Microsoft.Extensions.DependencyInjection;
using NetBB.System.EventBus.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Services
{
    public class MemoryCommandDispatcher(IServiceScopeFactory serviceScopeFactory, ITransactionManager transactionManager) : ICommandDispatcher
    {
        public async Task<R> SendCommand<C, R>(C cmd) where C : Command<R>
        {
            //var commandType = cmd.GetType();
            //var rType = typeof(R);
            //var targetType = typeof(ICommandHandler<,>).MakeGenericType(commandType, rType);
            using var scoped = serviceScopeFactory.CreateAsyncScope();
            var service = scoped.ServiceProvider.GetService<ICommandHandler<C, R>>();
            if (service == null)
            {
                throw new EventBusServiceException("No command handler found for command:" + cmd.GetType().Name);
            }
            else
            {
                DomainContainer domainContainer = PrepareDomainContainer(serviceScopeFactory);

                return await transactionManager.RunInTransaction<R>(async () =>
                {
                    var result = await service.HandleCommand(cmd, domainContainer);
                    await ((IEventDiaptcherInternal)domainContainer.EventDispatcher).RunEventHandling();
                    return result;
                });
            }
        }

        private DomainContainer PrepareDomainContainer(IServiceScopeFactory serviceScopeFactory)
        {
            IAggregateCrowd aggregateCrowd = new RequestBasedMemoryAggregateCrowd();
            IEventDispatcher eventDispatcher = new RequestBasedMemoryEventDispatcher(serviceScopeFactory, aggregateCrowd);
            DomainContainer domainContainer = new(
                AggregateCrowd: aggregateCrowd
                , EventDispatcher: eventDispatcher
                , FollowUpDispatcher: new OnceTimeMemoryFollowUpDispatcher(serviceScopeFactory, aggregateCrowd, eventDispatcher)
                );

            return domainContainer;
        }
    }
}

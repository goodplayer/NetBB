using Microsoft.CSharp.RuntimeBinder;
using NetBB.System.EventBus.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Services
{
    public static class DispatcherUtils
    {
        public static FromThis From(DomainContainer container) => new FromThis(container);
    }

    public sealed class FromThis
    {
        private DomainContainer _container;
        public FromThis(DomainContainer container)
        {
            _container = container;
        }
        /// <summary>
        /// Trigger an async event for future work
        /// </summary>
        /// <param name="event"></param>
        public async Task Trigger<T>(T @event) where T : Event
        {
            await this._container.EventDispatcher.Apply(@event);
        }
        /// <summary>
        /// <para>Trigger an event which followed by several sequential tasks</para>
        /// <para>Note: this will have an side effect that the provider may have knowledge on consumers</para>
        /// <para>Note2: this method requires
        ///         1. domain operation complete before this method in command/event handler in order for business integrity,
        ///         2. and all remote operations(like db and messaging) which requires the guarantee of business order should support transaction
        /// </para>
        /// </summary>
        /// <param name="event"></param>
        public async Task Following<T>(T @event) where T : Event
        {
            await this._container.FollowUpDispatcher.Apply(@event);
        }
        public async Task FollowUpOn<T>(IAggregate<T> aggregate)
        {
            await this._container.AggregateCrowd.FollowUpOn<T>(aggregate);
        }

        public async Task<R> Refer<T, R>(T identifier) where R : IAggregate<T>
        {
            return await this._container.AggregateCrowd.Refer<T, R>(identifier);
        }
    }

}

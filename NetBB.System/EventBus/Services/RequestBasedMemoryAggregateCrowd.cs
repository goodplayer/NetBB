using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetBB.System.EventBus.Models;

namespace NetBB.System.EventBus.Services
{
    public class RequestBasedMemoryAggregateCrowd : IAggregateCrowd
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, object>> store = new();
        public Task FollowUpOn<T>(IAggregate<T> aggregate)
        {
            var aggType = aggregate.GetType();
            var v = store.GetValueOrDefault(aggType, null) ?? store.AddOrUpdate(aggType, (t) => new ConcurrentDictionary<object, object>(), (t, old) => old);
            var id = aggregate.AggregateIdentifier();
            var obj = v.GetValueOrDefault(id, null) as IAggregate<T>;
            if (obj == null)
            {
                v[id] = aggregate;
                return Task.CompletedTask;
            }
            else
            {
                // chekc duplication for object type and id
                throw new EventBusServiceException("FollowUpOn duplicated object:" + aggType + " id:" + id);
            }
        }

        public Task<R> Refer<T, R>(T identifier) where R : IAggregate<T>
        {
            var resultType = typeof(R);

            var v = store[resultType];

            var obj = (R)v[identifier];

            return Task.FromResult(obj);
        }
    }
}

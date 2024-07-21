using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetBB.System.EventBus.Models;

namespace NetBB.System.EventBus.Services
{
    public interface IAggregateCrowd
    {
        /// <summary>
        /// FollowUpOn allows followups using exising aggregates via FollowUpDispatcher
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aggregate"></param>
        Task FollowUpOn<T>(IAggregate<T> aggregate);
        /// <summary>
        /// Refer will respond the aggregate for the given aggregate id and expected aggreate type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<R> Refer<T, R>(T identifier) where R : IAggregate<T>;
    }
}

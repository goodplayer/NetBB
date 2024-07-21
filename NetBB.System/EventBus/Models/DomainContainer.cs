using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetBB.System.EventBus.Services;

namespace NetBB.System.EventBus.Models
{
    public record DomainContainer(IAggregateCrowd AggregateCrowd, IEventDispatcher EventDispatcher, IFollowUpDispatcher FollowUpDispatcher)
    {
    }
}

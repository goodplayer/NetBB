using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Models
{
    public interface IAggregate<ID>
    {
        ID AggregateIdentifier();
    }
}

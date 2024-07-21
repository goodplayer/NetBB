using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Models
{
    public interface IQueryHandler<C, R> where C : Query<R>
    {
        Task<R> HandleQuery(C query);
    }
}

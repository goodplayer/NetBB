using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Services
{
    public interface ITransactionManager
    {
        public Task<R> RunInTransaction<R>(Func<Task<R>> f);
    }
}

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NetBB.Infrastructure.Repositories;
using NetBB.System.EventBus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Infrastructure.EventBusComponents
{
    public class TransactionManagerImpl(IServiceScopeFactory serviceScopeFactory) : ITransactionManager
    {
        public async Task<R> RunInTransaction<R>(Func<Task<R>> f)
        {
            using var scoped = serviceScopeFactory.CreateAsyncScope();
            var databasseContext = scoped.ServiceProvider.GetRequiredService<DatabaseContext>();
            var txn = await databasseContext.RawDatabase().BeginTransactionAsync();
            bool success = false;
            try
            {
                R r = await f();
                success = true;
                return r;
            }
            finally
            {
                if (success)
                {
                    await txn.CommitAsync();
                }
                else
                {
                    await txn.RollbackAsync();
                }
            }
        }
    }
}

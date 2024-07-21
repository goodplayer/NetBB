using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NetBB.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace NetBB.Infrastructure.Session
{
    public static class SessionPlugin
    {
        public static IServiceCollection AddDatabaseDistributedCache(this IServiceCollection services)
        {
            services.Add(ServiceDescriptor.Singleton<IDistributedCache, DatabaseDistributedCache>());
            return services;
        }
    }
    public class DatabaseDistributedCache(IServiceScopeFactory serviceScopeFactory, ILogger<DatabaseDistributedCache> _logger) : IDistributedCache
    {
        public static readonly int SESSION_EXPIRATION_TIME_IN_MILLIS = 24 * 60 * 60 * 1000;

        private long GetCurrentTimeInMillis()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        private bool IsExpired(long time)
        {
            return GetCurrentTimeInMillis() > time;
        }

        private long GetNewExpireTime()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() + SESSION_EXPIRATION_TIME_IN_MILLIS;
        }

        private async Task<R> CallTxn<R>(Func<DatabaseContext, Task<R>> f)
        {
            using IServiceScope scope = serviceScopeFactory.CreateAsyncScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            R r = await f(databaseContext);
            return r;
        }

        private async Task RunTxn(Func<DatabaseContext, Task> f)
        {
            using IServiceScope scope = serviceScopeFactory.CreateAsyncScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            await f(databaseContext);
        }

        public byte[]? Get(string key)
        {
            var r = CallTxn(databaseContext =>
            {
                var item = databaseContext.DatabaseCacheItems.Where(d => d.Key.Equals(key)).OrderByDescending(d => d.TimeStarted).FirstOrDefault();
                if (item == null || IsExpired(item.TimeExpired))
                {
                    return Task.FromResult<byte[]?>([]);
                }
                return Task.FromResult<byte[]?>(item.Value);
            });
            r.RunSynchronously();
            return r.Result;
        }

        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            return await CallTxn<byte[]?>(async databaseContext =>
            {
                var item = await databaseContext.DatabaseCacheItems.Where(d => d.Key.Equals(key)).OrderByDescending(d => d.TimeStarted).FirstOrDefaultAsync();
                if (item == null || IsExpired(item.TimeExpired))
                {
                    return [];
                }
                return item.Value;
            });
        }

        public void Refresh(string key)
        {
            var r = RunTxn(databaseContext =>
            {
                var item = databaseContext.DatabaseCacheItems.Where(d => d.Key.Equals(key)).OrderByDescending(d => d.TimeStarted).FirstOrDefault();
                if (item == null || IsExpired(item.TimeExpired)) // If expired then this key should not be refreshed
                {
                    return Task.CompletedTask;
                }
                item.TimeExpired = this.GetNewExpireTime();
                databaseContext.SaveChanges();
                return Task.CompletedTask;
            });
            r.RunSynchronously();
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            await RunTxn(async databaseContext =>
            {
                var item = await databaseContext.DatabaseCacheItems.Where(d => d.Key.Equals(key)).OrderByDescending(d => d.TimeStarted).FirstOrDefaultAsync();
                if (item == null || IsExpired(item.TimeExpired)) // If expired then this key should not be refreshed
                {
                    return;
                }
                item.TimeExpired = this.GetNewExpireTime();
                await databaseContext.SaveChangesAsync();
            });
        }

        public void Remove(string key)
        {
            var r = RunTxn(databaseContext =>
            {
                var items = databaseContext.DatabaseCacheItems.Where(d => d.Key.Equals(key));
                foreach (var item in items)
                {
                    databaseContext.DatabaseCacheItems.Remove(item);
                }
                databaseContext.SaveChanges();
                return Task.CompletedTask;
            });
            r.RunSynchronously();
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            await RunTxn(async databaseContext =>
            {
                var items = await databaseContext.DatabaseCacheItems.Where(d => d.Key.Equals(key)).ToArrayAsync();
                foreach (var item in items)
                {
                    databaseContext.DatabaseCacheItems.Remove(item);
                }
                await databaseContext.SaveChangesAsync();
            });
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var r = RunTxn(databaseContext =>
            {
                var cacheItem = databaseContext.DatabaseCacheItems.Where(i => i.Key.Equals(key)).OrderByDescending(i => i.TimeExpired).FirstOrDefault();
                if (cacheItem != null)
                {
                    cacheItem.Value = value;
                    cacheItem.TimeExpired = GetNewExpireTime();
                    databaseContext.SaveChanges();
                }
                else
                {
                    //TODO use options for expiration settings
                    //_logger.LogInformation("set session data:{} with options:{}", key, options);
                    DatabaseCacheItem item = new();
                    item.Key = key;
                    item.Value = value;
                    item.TimeStarted = GetCurrentTimeInMillis();
                    item.TimeExpired = GetNewExpireTime();
                    databaseContext.DatabaseCacheItems.Add(item);
                    databaseContext.SaveChanges();
                }
                return Task.CompletedTask;
            });
            r.RunSynchronously();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            await RunTxn(async databaseContext =>
            {
                //TODO use options for expiration settings
                //_logger.LogInformation("set session data:{} with options:{}", key, options);
                var cacheItem = await databaseContext.DatabaseCacheItems.Where(i => i.Key.Equals(key)).OrderByDescending(i => i.TimeExpired).FirstOrDefaultAsync();
                if (cacheItem != null)
                {
                    cacheItem.Value = value;
                    cacheItem.TimeExpired = GetNewExpireTime();
                    await databaseContext.SaveChangesAsync();
                }
                else
                {
                    DatabaseCacheItem item = new();
                    item.Key = key;
                    item.Value = value;
                    item.TimeStarted = GetCurrentTimeInMillis();
                    item.TimeExpired = GetNewExpireTime();
                    databaseContext.DatabaseCacheItems.Add(item);
                    await databaseContext.SaveChangesAsync();
                }
            });
        }
    }
}

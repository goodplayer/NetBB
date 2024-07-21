using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NetBB.Domain.Domains.User;
using NetBB.Infrastructure.Repositories;
using NetBB.System.EventBus.Services;
using System.Net.Sockets;

namespace NetBB.Sources.Components
{
    public class TicketStore(IServiceScopeFactory serviceScopeFactory, ILogger<TicketStore> _logger) : ITicketStore
    {
        private readonly int EXPIRE_DAYS = 30;
        private readonly int EXPIRE_DAYS_INCREMENTAL = 30+1;
        private readonly string USER_ID_KEY = "user_id";
        private readonly string LOGIN_ID_KEY = "login_id";

        public async Task RemoveAsync(string key)
        {
            _logger.LogInformation("=====>>>> Trigger RemoveAsync");
            using var scoped = serviceScopeFactory.CreateAsyncScope();
            using var databaseContext = scoped.ServiceProvider.GetRequiredService<DatabaseContext>();

            var items = databaseContext.AuthTickets.Where(w => w.AuthKey == key).ToHashSet();
            foreach (var item in items)
            {
                databaseContext.AuthTickets.Remove(item);
            }
            await databaseContext.SaveChangesAsync();
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            _logger.LogInformation("=====>>>> Trigger RenewAsync");
            var userId = ticket.Principal.Claims.Where(c => c.Type == USER_ID_KEY).FirstOrDefault();
            var loginId = ticket.Principal.Claims.Where(c => c.Type == LOGIN_ID_KEY).FirstOrDefault();
            if (userId == null || loginId == null)
            {
                throw new UnexpectedBusinessException("user id or/and login id is null");
            }

            using var scoped = serviceScopeFactory.CreateAsyncScope();
            using var databaseContext = scoped.ServiceProvider.GetRequiredService<DatabaseContext>();

            var oldAt = await databaseContext.AuthTickets
                .Where(a => a.AuthKey == key)
                .OrderByDescending(a => a.TimeExpired)
                .FirstOrDefaultAsync();
            if (oldAt != null)
            {
                if (oldAt.TimeExpired <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                {
                    // expired
                    return;
                }
                if (oldAt.UserId != userId.Value || oldAt.AuthScheme != ticket.AuthenticationScheme)
                {
                    // not current ticket owner
                    return;
                }

                if (ticket.Properties.ExpiresUtc != null)
                {
                    oldAt.TimeExpired = ticket.Properties.ExpiresUtc.Value.ToUnixTimeMilliseconds();
                }
                else
                {
                    _logger.LogWarning("ticket renew cannot get ExpiresUtc. Use default expire days instead.");
                    oldAt.TimeExpired = DateTimeOffset.Now.AddDays(EXPIRE_DAYS).ToUnixTimeMilliseconds();
                }
                oldAt.TicketValue = SerializeToBytes(ticket);
                await databaseContext.SaveChangesAsync();
            }
        }

        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            _logger.LogInformation("=====>>>> Trigger RetrieveAsync");
            using var scoped = serviceScopeFactory.CreateAsyncScope();
            using var databaseContext = scoped.ServiceProvider.GetRequiredService<DatabaseContext>();

            var oldAt = await databaseContext.AuthTickets
                .Where(a => a.AuthKey == key)
                .OrderByDescending(a => a.TimeExpired)
                .FirstOrDefaultAsync();
            if (oldAt != null)
            {
                if (oldAt.TimeExpired <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                {
                    // expired
                    databaseContext.AuthTickets.Remove(oldAt);
                    await databaseContext.SaveChangesAsync();
                    return null;
                }
                if (oldAt.TimeExpired <= DateTimeOffset.Now.AddDays(EXPIRE_DAYS).ToUnixTimeMilliseconds())
                {
                    // near expired, extend expiration date
                    oldAt.TimeExpired = DateTimeOffset.Now.AddDays(EXPIRE_DAYS_INCREMENTAL).ToUnixTimeMilliseconds();
                    await databaseContext.SaveChangesAsync();
                }
                return DeserializeFromBytes(oldAt.TicketValue);
            }

            return null;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            _logger.LogInformation("=====>>>> Trigger StoreAsync");
            var userId = ticket.Principal.Claims.Where(c => c.Type == USER_ID_KEY).FirstOrDefault();
            var loginId = ticket.Principal.Claims.Where(c => c.Type == LOGIN_ID_KEY).FirstOrDefault();
            if (userId == null || loginId == null)
            {
                throw new UnexpectedBusinessException("user id or/and login id is null");
            }

            using var scoped = serviceScopeFactory.CreateAsyncScope();
            using var databaseContext = scoped.ServiceProvider.GetRequiredService<DatabaseContext>();

            // query and check user id
            var oldAt = await databaseContext.AuthTickets
                .Where(a => a.AuthKey == loginId.Value)
                .OrderByDescending(a => a.TimeExpired)
                .FirstOrDefaultAsync();
            var decision = "new";
            if (oldAt != null)
            {
                if (oldAt.TimeExpired <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                {
                    decision = "new-delete"; // expired
                }
                else if (oldAt.UserId != userId.Value || oldAt.AuthScheme != ticket.AuthenticationScheme)
                {
                    // login id conflict, user not match, overwrite
                    // or auth scheme does not match, overwrite
                    decision = "new-overwrite";
                }
                else
                {
                    decision = "update"; // update old record
                }
            }
            else
            {
                decision = "new"; // new record
            }

            var at = new AuthTicket();
            at.AuthScheme = ticket.AuthenticationScheme;
            at.AuthKey = loginId.Value;
            at.LoginId = loginId.Value;
            if (ticket.Properties.ExpiresUtc != null)
            {
                at.TimeExpired = ticket.Properties.ExpiresUtc.Value.ToUnixTimeMilliseconds();
            }
            else
            {
                _logger.LogWarning("ticket store cannot get ExpiresUtc. Use default expire days instead.");
                at.TimeExpired = DateTimeOffset.Now.AddDays(EXPIRE_DAYS).ToUnixTimeMilliseconds();
            }
            at.UserId = userId.Value;
            at.TimeStarted = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            at.TicketValue = SerializeToBytes(ticket);

            switch (decision)
            {
                case "new":
                    databaseContext.AuthTickets.Add(at);
                    break;
                case "new-delete":
                    databaseContext.AuthTickets.Remove(oldAt);
                    databaseContext.AuthTickets.Add(at);
                    break;
                case "new-overwrite":
                    databaseContext.AuthTickets.Remove(oldAt);
                    databaseContext.AuthTickets.Add(at);
                    break;
                case "update":
                    databaseContext.AuthTickets.Remove(oldAt);
                    databaseContext.AuthTickets.Add(at);
                    break;
                default:
                    throw new UnexpectedBusinessException("unknown decision:" + decision + " while StoreAsync ticket");
            }

            await databaseContext.SaveChangesAsync();

            return loginId.Value;
        }

        private byte[] SerializeToBytes(AuthenticationTicket source) => TicketSerializer.Default.Serialize(source);

        private AuthenticationTicket? DeserializeFromBytes(byte[] source)
            => source == null ? null : TicketSerializer.Default.Deserialize(source);
    }
}

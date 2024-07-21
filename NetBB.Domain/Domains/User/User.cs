using NetBB.System.EventBus.Models;
using NetBB.System.EventBus.Services;
using BC = BCrypt.Net.BCrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.User
{
    public class User : IAggregate<long>
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? NickName { get; set; }
        public string? Email { get; set; }

        public long TimeCreated { get; set; }
        public long TimeUpdated { get; set; }

        public long AggregateIdentifier()
        {
            return this.Id;
        }

        internal static async Task<User> CreateUser(FromThis fromThis
            , string userName
            , string password
            , string nickName
            , string email
            , GenerateUserId generateUserId)
        {
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            User user = new()
            {
                UserName = userName,
                Password = BC.EnhancedHashPassword(password),
                NickName = nickName,
                Email = email,
                Id = await generateUserId(), // guarantee ID when domain object is created
                TimeCreated = now,
                TimeUpdated = now
            };

            await fromThis.FollowUpOn(user);
            await fromThis.Trigger(new UserRegisteredEvent(user.Id, user.UserName));

            return user;
        }

        internal async Task<bool> Login(FromThis fromThis, string password)
        {
            var matched = BC.EnhancedVerify(password, this.Password);
            if (!matched)
            {
                return false;
            }
            await fromThis.FollowUpOn(this);
            await fromThis.Trigger(new UserLoginedEvent(this.UserName, this.NickName, this.Id));
            return true;
        }
    }
}

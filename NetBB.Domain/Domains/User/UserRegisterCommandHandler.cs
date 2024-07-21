using NetBB.System.EventBus.Models;
using static NetBB.System.EventBus.Services.DispatcherUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.User
{
    public record UserRegisterCommand(string UserName, string Password, string NickName, string Email) : Command<UserRegisterResult>
    {
    }
    public record UserRegisterResult(long userId, string username)
    {
    }

    public record UserRegisteredEvent(long Id, string UserName) : Event
    {
    }

    public class UserRegisterCommandHandler(IUserRepository userRepository) : ICommandHandler<UserRegisterCommand, UserRegisterResult>
    {
        public async Task<UserRegisterResult> HandleCommand(UserRegisterCommand command, DomainContainer container)
        {
            var user = await User.CreateUser(From(container)
                , command.UserName
                , command.Password
                , command.NickName
                , command.Email
                , userRepository.GenerateUserId);

            await userRepository.Save(user);

            return new UserRegisterResult(user.Id, user.UserName);
        }
    }
}

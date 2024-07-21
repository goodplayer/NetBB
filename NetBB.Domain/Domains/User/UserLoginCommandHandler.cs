using NetBB.System.EventBus.Models;
using static NetBB.System.EventBus.Services.DispatcherUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.User
{
    public record UserLoginCommand(string username, string password) : Command<UserLoginResult>
    {
    }

    public record UserLoginResult(bool logined, bool userFound, long userId = 0, string nickName = "", string roleName = "")
    {
    }

    public record UserLoginedEvent(string username, string nickname, long userId) : Event
    {
    }

    public class UserLoginCommandHandler(IUserRepository userRepository) : ICommandHandler<UserLoginCommand, UserLoginResult>
    {
        public async Task<UserLoginResult> HandleCommand(UserLoginCommand command, DomainContainer container)
        {
            User? user = await userRepository.LoadUserByUserName(command.username);
            if (user == null)
            {
                return new UserLoginResult(logined: false, userFound: false);
            }

            bool logined = await user.Login(From(container), command.password);
            return new UserLoginResult(logined: logined, userFound: true, userId: user.Id, nickName: user.NickName, roleName: "normal");
        }
    }
}

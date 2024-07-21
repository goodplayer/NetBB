using NetBB.System.EventBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.User
{
    public class UserRegisteredNotification : IEventHandler<UserRegisteredEvent>
    {
        public async Task HandleEvent(UserRegisteredEvent e, DomainContainer container)
        {
            //Console.WriteLine("user registered!!!!!!!");
        }
    }
}

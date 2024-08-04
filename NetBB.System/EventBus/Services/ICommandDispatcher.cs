using NetBB.System.EventBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Services
{
    public interface ICommandDispatcher
    {
        Task<R> SendCommand<C, R>(C cmd) where C : Command<R>;

        Task<R> SendQuery<Q, R>(Q query) where Q : Query<R>;
    }
}

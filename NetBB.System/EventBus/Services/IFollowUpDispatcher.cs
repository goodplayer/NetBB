using NetBB.System.EventBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Services
{
    public interface IFollowUpDispatcher
    {
        Task Apply<T>(T @event) where T : Event;
    }
}

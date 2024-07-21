using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Services
{
    public class EventBusServiceException : Exception
    {
        public EventBusServiceException() : base() { }
        public EventBusServiceException(string message) : base(message) { }
        public EventBusServiceException(string message, Exception inner) : base(message, inner) { }
    }
}

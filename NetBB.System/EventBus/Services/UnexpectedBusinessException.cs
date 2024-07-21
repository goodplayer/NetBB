using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.System.EventBus.Services
{
    public class UnexpectedBusinessException : Exception
    {
        public UnexpectedBusinessException() : base() { }
        public UnexpectedBusinessException(string message) : base(message) { }
        public UnexpectedBusinessException(string message, Exception inner) : base(message, inner) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Infrastructure.Repositories
{
    public class AuthTicket
    {
        public long Id { get; set; }
        public string AuthScheme { get; set; }
        public string AuthKey { get; set; }
        public byte[] TicketValue { get; set; }
        public string UserId { get; set; }
        public string LoginId { get; set; }
        public long TimeStarted { get; set; }
        public long TimeExpired { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Infrastructure.Repositories
{
    public class DatabaseCacheItem
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public byte[] Value { get; set; }
        public long TimeStarted { get; set; }
        public long TimeExpired { get; set; }
    }
}

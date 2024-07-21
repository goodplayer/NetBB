using NetBB.Domain.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.User
{
    public delegate Task<long> GenerateUserId();
    public interface IUserRepository : IRepository
    {
        Task Save(User user);

        Task<long> GenerateUserId();

        Task<User?> LoadUserByUserName(string userName);
    }
}

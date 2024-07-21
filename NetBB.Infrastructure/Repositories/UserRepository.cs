using Microsoft.EntityFrameworkCore;
using NetBB.Domain.Domains.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Infrastructure.Repositories
{
    public class UserRepository(DatabaseContext context) : IUserRepository
    {
        public async Task<long> GenerateUserId()
        {
            var list = await context.Database.SqlQuery<long>($"SELECT nextval('user_id_seq')").ToListAsync();
            return list.First();
        }

        public async Task<User?> LoadUserByUserName(string userName)
        {
            return await context.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
        }

        public async Task Save(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}

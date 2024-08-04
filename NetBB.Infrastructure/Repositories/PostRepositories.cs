using Microsoft.EntityFrameworkCore;
using NetBB.Domain.Domains.Post;
using NetBB.Domain.Domains.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Infrastructure.Repositories
{
    public class PostRepository(DatabaseContext context) : IPostRepository
    {
        public async Task<long> GeneratePostId()
        {
            var list = await context.Database.SqlQuery<long>($"SELECT nextval('post_id_seq')").ToListAsync();
            return list.First();
        }

        public async Task<Post?> GetById(long id)
        {
            return await context.Posts.FindAsync(id);
        }

        public async Task<List<Post>> ListAllPosts()
        {
            return await context.Posts.OrderByDescending(p => p.TimeCreated).ToListAsync();
        }

        public async Task Save(Post post)
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        }

        public async Task SaveUpdate()
        {
            await context.SaveChangesAsync();
        }
    }

    public class PostHistoryRepository(DatabaseContext context) : IPostHistoryRepository
    {
        public async Task<long> GeneratePostHistoryId()
        {
            var list = await context.Database.SqlQuery<long>($"SELECT nextval('post_history_id_seq')").ToListAsync();
            return list.First();
        }

        public async Task Save(PostHistory postHistory)
        {
            context.PostHistories.Add(postHistory);
            await context.SaveChangesAsync();
        }
    }
}

using NetBB.Domain.Infrastructures;
using NetBB.System.EventBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.Post
{
    public delegate Task<long> GeneratePostId();
    public interface IPostRepository : IRepository
    {
        Task<long> GeneratePostId();

        Task Save(Post post);

        Task<List<Post>> ListAllPosts();

        Task<Post?> GetById(long id);

        Task SaveUpdate();
    }
    
    public delegate Task<long> GeneratePostHistoryId();
    public interface IPostHistoryRepository : IRepository
    {
        Task<long> GeneratePostHistoryId();

        Task Save(PostHistory postHistory);
    }

    public record PostListQuery() : Query<List<PostListItem>>
    {
    }
    public record PostListItem(long postId, long userId, string title, string content, long timeUpdated, long timeCreated)
    {

    }

    public record PostByIdQuery(long postId) : Query<PostItem>
    {
    }
    public record PostItem(bool found = false, long postId = 0, long userId = 0, string title = "", string content = "", string contentType = "", long timeUpdated = 0, long timeCreated = 0)
    {
    }
}

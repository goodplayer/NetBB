using NetBB.Domain.Domains.Post;
using NetBB.System.EventBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.Queries
{
    public class PostListQueryHandler(IPostRepository postRepository) : IQueryHandler<PostListQuery, List<PostListItem>>
    {
        public async Task<List<PostListItem>> HandleQuery(PostListQuery query)
        {
            var result = new List<PostListItem>();
            var queryResult = await postRepository.ListAllPosts();
            foreach (var post in queryResult)
            {
                result.Add(new PostListItem(post.PostId, post.AuthorId, post.PostTitle, post.PostContent, post.TimeUpdated, post.TimeCreated));
            }
            return result;
        }
    }
}

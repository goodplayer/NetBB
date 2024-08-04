using NetBB.Domain.Domains.Post;
using NetBB.System.EventBus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.Queries
{
    public class PostByIdQueryHandler(IPostRepository postRepository) : IQueryHandler<PostByIdQuery, PostItem>
    {
        public async Task<PostItem> HandleQuery(PostByIdQuery query)
        {

            var post = await postRepository.GetById(query.postId);
            if (post == null)
            {
                return new PostItem();
            }

            return new PostItem(
                found: true,
                postId: post.PostId,
                userId: post.AuthorId,
                title: post.PostTitle,
                content: post.PostContent,
                contentType: post.PostType,
                timeUpdated: post.TimeUpdated,
                timeCreated: post.TimeCreated
                );
        }
    }
}

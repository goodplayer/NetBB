using NetBB.System.EventBus.Models;
using NetBB.System.EventBus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.Post
{
    public class Post : IAggregate<long>
    {
        public long PostId { get; set; }

        public long AuthorId { get; set; }

        public string PostType { get; set; } = "";

        public string PostTitle { get; set; } = "";

        public string PostContent { get; set; } = "";

        public long TimeCreated { get; set; }
        public long TimeUpdated { get; set; }

        public long AggregateIdentifier()
        {
            return PostId;
        }

        public static async Task<Post> CreatePost(FromThis fromThis
            , string title
            , string content
            , long userId
            , string postContentType
            , GeneratePostId generatePostId)
        {
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var post = new Post
            {
                PostId = await generatePostId(),
                AuthorId = userId,
                PostType = postContentType,
                PostTitle = title,
                PostContent = content,
                TimeCreated = now,
                TimeUpdated = now,
            };

            await fromThis.FollowUpOn(post);
            await fromThis.Following(new NewPostEvent(postId: post.PostId, userId: post.AuthorId, title: post.PostTitle, content: post.PostContent, postContentType: post.PostType));

            return post;
        }

        public async Task Edit(FromThis fromThis, string postType, string title, string content, long updateAuthorId)
        {
            this.PostType = postType;
            this.PostTitle = title;
            this.PostContent = content;
            this.TimeUpdated = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            await fromThis.FollowUpOn(this);
            await fromThis.Following(new EditPostEvent(this.PostId, updateAuthorId, this.PostTitle, this.PostContent, this.PostType, this.TimeUpdated));
        }
    }
}

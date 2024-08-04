using NetBB.System.EventBus.Models;
using NetBB.System.EventBus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.Post
{
    public class PostHistory : IAggregate<long>
    {
        public long PostHistoryId { get; set; }
        public long PostId { get; set; }
        public long AuthorId { get; set; }
        public string PostContentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public long TimeCreated { get; set; }

        public long AggregateIdentifier()
        {
            return PostHistoryId;
        }

        public static async Task<PostHistory> Create(FromThis fromThis
            , long postId
            , long authorId
            , string PostContentType
            , string Title
            , string Content
            , GeneratePostHistoryId generatePostHistoryId)
        {
            var postHistoryId = await generatePostHistoryId();
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var postHistory = new PostHistory
            {
                PostId = postId,
                PostHistoryId = postHistoryId,
                AuthorId = authorId,
                PostContentType = PostContentType,
                Title = Title,
                Content = Content,
                TimeCreated = now,
            };

            await fromThis.FollowUpOn(postHistory);
            await fromThis.Following(new NewPostHistoryEvent(postHistoryId, postId));

            return postHistory;
        }
    }
}

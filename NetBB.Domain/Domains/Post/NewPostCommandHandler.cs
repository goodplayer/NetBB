using NetBB.System.EventBus.Models;
using NetBB.System.EventBus.Services;
using static NetBB.System.EventBus.Services.DispatcherUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.Post
{
    public record NewPostCommand(string title, string content, string postContentType, long userId) : Command<NewPostResult>
    {

    }

    public record NewPostResult()
    {

    }

    public record NewPostEvent(long postId, long userId, string title, string content, string postContentType) : Event
    {

    }

    public class NewPostCommandHandler(IPostRepository postRepository) : ICommandHandler<NewPostCommand, NewPostResult>
    {
        public async Task<NewPostResult> HandleCommand(NewPostCommand command, DomainContainer container)
        {

            var post = await Post.CreatePost(From(container)
                , command.title
                , command.content
                , command.userId
                , command.postContentType
                , postRepository.GeneratePostId
                );

            await postRepository.Save(post);

            var result = new NewPostResult();
            return result;
        }
    }
}

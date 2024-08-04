using NetBB.System.EventBus.Models;
using static NetBB.System.EventBus.Services.DispatcherUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace NetBB.Domain.Domains.Post
{
    public record EditPostCommand(long postId, string title, string content, string postContentType, long userId) : Command<EditPostResult>
    {
    }

    public record EditPostResult(IDictionary<string, string> errors)
    {
    }

    public record EditPostEvent(long postId, long userId, string title, string content, string postContentType, long timeUpdated) : Event
    {
    }

    public class EditPostCommandHandler(IPostRepository postRepository) : ICommandHandler<EditPostCommand, EditPostResult>
    {
        public async Task<EditPostResult> HandleCommand(EditPostCommand command, DomainContainer container)
        {
            var post = await postRepository.GetById(command.postId);
            if (post == null)
            {
                return new EditPostResult(new Dictionary<string, string>()
                {
                    {"post_not_found", "文章不存在" }
                });
            }
            await post.Edit(From(container), command.postContentType, command.title, command.content, command.userId);

            await postRepository.SaveUpdate();

            return new EditPostResult(ImmutableDictionary<string, string>.Empty);
        }
    }
}

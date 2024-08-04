using NetBB.System.EventBus.Models;
using static NetBB.System.EventBus.Services.DispatcherUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Domain.Domains.Post
{
    public record NewPostHistoryEvent(long postHistoryId, long postId) : Event
    {
    }

    public class CreatingPostHistoryAfterNewPostHandler(IPostHistoryRepository postHistoryRepository) : IEventHandler<NewPostEvent>
    {
        public async Task HandleEvent(NewPostEvent ev, DomainContainer container)
        {
            var postHistory = await PostHistory.Create(From(container)
                , ev.postId, ev.userId, ev.postContentType, ev.title, ev.content, postHistoryRepository.GeneratePostHistoryId);
            await postHistoryRepository.Save(postHistory);
        }
    }

    public class CreatingPostHistoryAfterEditPostHandler(IPostHistoryRepository postHistoryRepository) : IEventHandler<EditPostEvent>
    {
        public async Task HandleEvent(EditPostEvent ev, DomainContainer container)
        {
            var postHistory = await PostHistory.Create(From(container)
                , ev.postId, ev.userId, ev.postContentType, ev.title, ev.content, postHistoryRepository.GeneratePostHistoryId);
            await postHistoryRepository.Save(postHistory);
        }
    }
}

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetBB.Domain.Domains.Post;
using NetBB.Sources.EnhancedWeb;
using NetBB.System.EventBus.Services;

namespace NetBB.Pages.Admin.Posts
{
    public class PostIndexModel(IPostRepository postRepository, ICommandDispatcher commandDispatcher)
        : EnhancedRazorPageModel
    {
        public async Task<IActionResult> OnGet()
        {
            if (!IsLogin())
            {
                return await SendLoginRedirection();
            }

            var posts = await commandDispatcher.SendQuery<PostListQuery, List<PostListItem>>(new PostListQuery());
            var postList = posts.Select(post => new
            {
                post_id = post.postId,
                title = post.title,
                last_update_time = post.timeUpdated,
            }).ToList();

            AddRenderItem("posts", postList);

            return PrepareRenderPage();
        }

        private IActionResult PrepareRenderPage()
        {
            PrepareRenderLoginStatus();
            return Page();
        }
    }
}
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetBB.Domain.Domains.Post;
using NetBB.Sources.Constants;
using NetBB.Sources.EnhancedWeb;
using NetBB.System.EventBus.Services;

namespace NetBB.Pages.Admin.Posts
{
    public class NewPostModel(ILogger<NewPostModel> _logger
        , IAntiforgery antiforgery
        , ICommandDispatcher commandDispatcher
        ) : EnhancedRazorPageModel
    {
        [BindProperty(Name = "title")]
        public string Title { get; set; }
        [BindProperty(Name = "content")]
        public string Content { get; set; }
        [BindProperty(Name = "content_type")]
        public string ContentType { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsLogin())
            {
                return RedirectToPage("/user/login", new { });
            }

            return PrepareRenderPage(antiforgery);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsLogin())
            {
                return RedirectToPage("/user/login", new { });
            }

            if (!AllowedPostContentType.ContentTypes.Contains(ContentType))
            {
                AddErrorInfo("content_type_unknown", "文本格式未知");
                return PrepareRenderPage(antiforgery);
            }

            Title = TrimInputOrEmpty(Title);
            Content = TrimInputOrEmpty(Content);
            if (string.IsNullOrEmpty(Title)) AddErrorInfo("title_empty", "标题为空");
            if (string.IsNullOrEmpty(Content)) AddErrorInfo("content_empty", "正文为空");
            if (HasErrorInfo())
            {
                return PrepareRenderPage(antiforgery);
            }

            var userId = GetLoginedUserId();

            // save post
            var newPostCommand = new NewPostCommand(title: Title, content: Content, postContentType: ContentType, userId: userId);
            var result = await commandDispatcher.SendCommand<NewPostCommand, NewPostResult>(newPostCommand);

            return Redirect("/admin/posts");
        }

        private IActionResult PrepareRenderPage(IAntiforgery antiforgery)
        {
            PrepareRenderLoginStatus();
            SetupAntiforgeryToken(antiforgery);
            return Page();
        }
    }
}

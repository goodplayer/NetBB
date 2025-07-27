using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetBB.Domain.Domains.Post;
using NetBB.Sources.Constants;
using NetBB.Sources.EnhancedWeb;
using NetBB.System.EventBus.Services;

namespace NetBB.Pages.Admin.Posts
{
    public class EditPostModel(IAntiforgery antiforgery
        , ICommandDispatcher dispatcher) : EnhancedRazorPageModel
    {

        [BindProperty(Name = "title")]
        public string Title { get; set; }
        [BindProperty(Name = "content")]
        public string Content { get; set; }
        [BindProperty(Name = "content_type")]
        public string ContentType { get; set; }

        public async Task<IActionResult> OnGetAsync(long PostId)
        {
            if (!IsLogin())
            {
                return await SendLoginRedirection();
            }

            if (PostId <= 0) AddErrorInfo("post_id_invalid", "���±�Ų��Ϸ�");
            if (HasErrorInfo())
            {
                return PrepareRenderPage(antiforgery);
            }

            var result = await dispatcher.SendQuery<PostByIdQuery, PostItem>(new PostByIdQuery(PostId));

            if (result.found)
            {
                AddRenderItem("post_id", result.postId);
                AddRenderItem("title", result.title);
                AddRenderItem("content", result.content);
            }

            return PrepareRenderPage(antiforgery);
        }

        public async Task<IActionResult> OnPostAsync(long PostId)
        {
            if (!IsLogin())
            {
                return await SendLoginRedirection();
            }

            if (!AllowedPostContentType.ContentTypes.Contains(ContentType))
            {
                AddErrorInfo("content_type_unknown", "�ı���ʽδ֪");
                return PrepareRenderPage(antiforgery);
            }

            Title = TrimInputOrEmpty(Title);
            Content = TrimInputOrEmpty(Content);
            if (string.IsNullOrEmpty(Title)) AddErrorInfo("title_empty", "����Ϊ��");
            if (string.IsNullOrEmpty(Content)) AddErrorInfo("content_empty", "����Ϊ��");
            if (PostId <= 0) AddErrorInfo("post_id_invalid", "���±�Ų��Ϸ�");
            if (HasErrorInfo())
            {
                return PrepareRenderPage(antiforgery);
            }

            var userId = GetLoginedUserId();

            // save post
            var editPostCommand = new EditPostCommand(postId: PostId, title: Title, content: Content, postContentType: ContentType, userId: userId);
            var result = await dispatcher.SendCommand<EditPostCommand, EditPostResult>(editPostCommand);
            if (result.errors.Count > 0)
            {
                foreach (var error in result.errors)
                {
                    AddErrorInfo(error.Key, error.Value);
                }
                return PrepareRenderPage(antiforgery);
            }

            return Redirect("/admin/posts");
        }

        private IActionResult PrepareRenderPage(IAntiforgery antiforgery)
        {
            SetupAntiforgeryToken(antiforgery);
            PrepareRenderLoginStatus();
            return Page();
        }
    }
}

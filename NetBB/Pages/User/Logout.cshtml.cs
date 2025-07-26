using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetBB.Sources.EnhancedWeb;

namespace NetBB.Pages.User
{
    public class LogoutModel(ILogger<LogoutModel> logger) : EnhancedRazorPageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            //RequestHeaders header = HttpContext.Request.GetTypedHeaders();
            //string uriReferer = header.Referer?.ToString() ?? "";
            //logger.LogError("========>>> referer: {}", uriReferer);

            await RevokeLoginStatus();

            return new RedirectResult("/");
        }
    }
}

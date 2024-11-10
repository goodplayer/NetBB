using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetBB.Domain.Domains.User;
using NetBB.Sources.EnhancedWeb;
using NetBB.Sources.Components;
using NetBB.System.EventBus.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using NetBB.Sources.Utilities;
using Microsoft.AspNetCore.Http.Headers;

namespace NetBB.Pages.User
{
    public class LoginModel(ILogger<LoginModel> logger
        , IAntiforgery antiforgery
        , ICommandDispatcher commandDispatcher
        , LongIdGenerator longIdGenerator
        ) : EnhancedRazorPageModel
    {

        [BindProperty(Name = "username")]
        public string UserName { get; set; }
        [BindProperty(Name = "password")]
        public string Password { get; set; }

        public async Task OnGetAsync()
        {
            //TODO login redirect to original page
            //RequestHeaders header = HttpContext.Request.GetTypedHeaders();
            //string uriReferer = header.Referer?.ToString() ?? "";
            //logger.LogError("========>>> login referer: {}", uriReferer);

            // prepare form submission token
            SetupAntiforgeryToken(antiforgery);

            PrepareRenderLoginStatus();
        }

        public async Task OnPostAsync()
        {
            var command = new UserLoginCommand(UserName, Password);
            var result = await commandDispatcher.SendCommand<UserLoginCommand, UserLoginResult>(command);

            if (result.logined)
            {
                await SetLoginStatus(result.userId, result.nickName, result.roleName, longIdGenerator.GenerateId());
                SetPageMode("logined");
                DirectlyRenderLogined(result.nickName, result.userId);
            }
            else
            {
                // prepare form submission token
                SetupAntiforgeryToken(antiforgery);
                ErrorInfo["login_failed"] = "用户名或密码不正确";
                PrepareRenderLoginStatus();
            }
        }
    }
}

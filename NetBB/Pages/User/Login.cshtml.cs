using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetBB.Domain.Domains.User;
using NetBB.Sources.EnhancedWeb;
using NetBB.Sources.Session;
using NetBB.System.EventBus.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using NetBB.Sources.Utilities;

namespace NetBB.Pages.User
{
    [EnableSession]
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
            // prepare form submission token
            RenderJson["request_verification_token"] = GetAntiforgeryToken(antiforgery);

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
                RenderLogined(result.nickName, result.userId);
            }
            else
            {
                // prepare form submission token
                RenderJson["request_verification_token"] = GetAntiforgeryToken(antiforgery);
                ErrorInfo["login_failed"] = "用户名或密码不正确";
                PrepareRenderLoginStatus();
            }
        }
    }
}

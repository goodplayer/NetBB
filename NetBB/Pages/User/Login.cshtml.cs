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
using Microsoft.Extensions.Primitives;

namespace NetBB.Pages.User
{
    public class LoginModel(
        ILogger<LoginModel> logger,
        IAntiforgery antiforgery,
        ICommandDispatcher commandDispatcher,
        LongIdGenerator longIdGenerator
    ) : EnhancedRazorPageModel
    {
        [BindProperty(Name = "username")] public string UserName { get; set; }
        [BindProperty(Name = "password")] public string Password { get; set; }

        private StringValues? ExtractRedirect()
        {
            var redirect = Request.Query["redirect"];
            if (string.IsNullOrEmpty(redirect))
            {
                return null;
            }
            else if (!redirect.ToString().StartsWith('/')) // block insecure redirect
            {
                return null;
            }
            else
            {
                return redirect;
            }
        }

        public async Task OnGetAsync()
        {
            // prepare form submission token
            SetupAntiforgeryToken(antiforgery);

            // add redirect url to page for submission
            var redirect = ExtractRedirect();
            if (redirect != (StringValues?)(null))
            {
                AddRenderItem("redirect", redirect.ToString());
            }

            PrepareRenderLoginStatus();
        }

        public async Task OnPostAsync()
        {
            var command = new UserLoginCommand(UserName, Password);
            var result = await commandDispatcher.SendCommand<UserLoginCommand, UserLoginResult>(command);

            // add redirect url to page for submission
            var redirect = ExtractRedirect();
            if (redirect != (StringValues?)(null))
            {
                AddRenderItem("redirect", redirect.ToString());
            }

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
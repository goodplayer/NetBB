using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetBB.Domain.Domains.User;
using NetBB.Sources.EnhancedWeb;
using NetBB.Sources.Components;
using NetBB.System.EventBus.Services;
using System.Collections.Concurrent;

namespace NetBB.Pages.User
{
    public class RegisterModel(ILogger<RegisterModel> logger
        , ICommandDispatcher commandDispatcher
        , IAntiforgery antiforgery) : EnhancedRazorPageModel
    {
        [BindProperty(Name = "username")]
        public string UserName { get; set; }
        [BindProperty(Name = "password")]
        public string Password { get; set; }
        [BindProperty(Name = "confirmpassword")]
        public string ConfirmPassword { get; set; }
        [BindProperty(Name = "email")]
        public string Email { get; set; }
        [BindProperty(Name = "nickname")]
        public string NickName { get; set; }

        public async Task OnGetAsync()
        {
            // prepare form submission token
            SetupAntiforgeryToken(antiforgery);

            PrepareRenderLoginStatus();
        }

        public async Task OnPostAsync()
        {
            // validate form fields otherwise respond fail items to user
            if (!Password.Equals(ConfirmPassword))
            {
                ErrorInfo["password_mismatch"] = "√‹¬Î≤ª∆•≈‰";
                SetupAntiforgeryToken(antiforgery);
                return;
            }

            // do register
            var result = await commandDispatcher.SendCommand<UserRegisterCommand, UserRegisterResult>(new UserRegisterCommand(UserName, Password, NickName, Email));

            PrepareRenderLoginStatus();
            // render register success page
            SetPageMode("registered");
        }
    }
}

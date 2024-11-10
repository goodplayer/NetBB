using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetBB.System.EventBus.Services;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace NetBB.Sources.EnhancedWeb
{
    public abstract class EnhancedRazorPageModel : PageModel
    {
        private readonly string CLAIM_NICKNAME = "nickname";
        private readonly string CLAIM_USER_ID = "user_id";

        public string PageMode = "default";
        public IDictionary<string, object> RenderJson { get; } = new ConcurrentDictionary<string, object>();
        public IDictionary<string, object> ErrorInfo { get; } = new ConcurrentDictionary<string, object>();
        public IDictionary<string, object> RenderObject { get; } = new ConcurrentDictionary<string, object>();
        public EnhancedRazorPageModel()
        {
            RenderJson["error_info"] = ErrorInfo;
            RenderJson["render_object"] = RenderObject;
        }

        public void SetupAntiforgeryToken(IAntiforgery antiforgery)
        {
            RenderJson["request_verification_token"] = GetAntiforgeryToken(antiforgery);
        }

        public string GetAntiforgeryToken(IAntiforgery antiforgery)
        {
            return antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
        }

        public long GetLoginedUserId()
        {
            string? userId = HttpContext.User.Claims.Where(c => c.Type == CLAIM_USER_ID).FirstOrDefault()?.Value;
            if (userId == null)
            {
                throw new UnexpectedBusinessException("no login user");
            }
            long number = long.Parse(userId);
            if (number <= 0)
            {
                throw new UnexpectedBusinessException("zero or negative user id");
            }
            return number;
        }

        public async Task SetLoginStatus(long userId, string nickname, string roleName, string loginId)
        {
            var claims = new List<Claim>
            {
                new Claim(CLAIM_NICKNAME, nickname),
                new Claim(CLAIM_USER_ID, userId.ToString()),
                new Claim("login_id", loginId),
                new Claim(ClaimTypes.Role, roleName),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // enable persistent cookie, meaning set-cookie from response for browser
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60 * 24),
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        public async Task RevokeLoginStatus()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public bool IsLogin()
        {
            return HttpContext.User.Identity?.IsAuthenticated ?? false;
        }

        public void PrepareRenderLoginStatus()
        {
            var loginObj = new Dictionary<string, object>();
            loginObj["is_logined"] = IsLogin();
            loginObj["nickname"] = HttpContext.User.Claims.Where(c => c.Type == CLAIM_NICKNAME).FirstOrDefault()?.Value ?? "";
            loginObj["user_id"] = HttpContext.User.Claims.Where(c => c.Type == CLAIM_USER_ID).FirstOrDefault()?.Value ?? "";
            RenderJson["login"] = loginObj;
        }

        public void DirectlyRenderLogined(string nickname, long userId)
        {
            var loginObj = new Dictionary<string, object>();
            loginObj["is_logined"] = true;
            loginObj["nickname"] = nickname;
            loginObj["user_id"] = userId.ToString();
            RenderJson["login"] = loginObj;
        }

        public string TrimInputOrEmpty(string input)
        {
            if (input == null) return "";
            return input.Trim();
        }

        public void AddErrorInfo(string key, string message)
        {
            ErrorInfo[key] = message;
        }

        public bool HasErrorInfo()
        {
            return ErrorInfo.Count > 0;
        }

        public void SetPageMode(string pageMode)
        {
            if (pageMode == null)
            {
                throw new UnexpectedBusinessException("page mode is null");
            }
            pageMode = pageMode.Trim();
            if (pageMode.Length == 0)
            {
                throw new UnexpectedBusinessException("page mode is empty");
            }
            PageMode = pageMode;
        }

        public void AddRenderItem(string key, object value)
        {
            RenderObject[key] = value;
        }
    }
}

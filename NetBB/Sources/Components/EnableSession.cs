using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBB.Sources.Components
{
    // EnableSession attribute
    // used on class
    public class EnableSession : Attribute
    {
    }

    public class AutoManageSessionViaAttribute
    {
        private readonly RequestDelegate _next;

        public AutoManageSessionViaAttribute(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var hasEnableSession = context.GetEndpoint()?.Metadata.Any(m => m is EnableSession) ?? false;

            if (hasEnableSession)
            {
                await context.Session.LoadAsync();
            }
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
            if (hasEnableSession)
            {
                await context.Session.CommitAsync();
            }
        }
    }
}

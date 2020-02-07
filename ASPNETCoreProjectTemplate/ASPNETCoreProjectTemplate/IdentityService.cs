using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ASPNETCoreProjectTemplate
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor context;

        public IdentityService(IHttpContextAccessor context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int GetAppUserId()
        {
            var appUserId = context.HttpContext.User.FindFirstValue("AppUserId");
            if (!int.TryParse(appUserId, out int value) || value < 1)
            {
                throw new InvalidOperationException("Invalid User Id");
            }
            return value;
        }

        public string GetUserId()
        {
            return context.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public string GetUserName()
        {
            return context.HttpContext.User.Identity.Name;
        }
    }
}
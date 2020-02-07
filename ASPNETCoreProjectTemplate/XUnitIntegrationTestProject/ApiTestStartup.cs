using ASPNETCoreProjectTemplate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace XUnitIntegrationTestProject
{
    public class ApiTestStartup : Startup
    {
        public ApiTestStartup(IConfiguration env) : base(env)
        {
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseMiddleware<TestAuthMiddleware>();
        }
    }

    public class ApiWithUserTestStartup : Startup
    {
        protected readonly IEnumerable<Claim> Claims;

        public ApiWithUserTestStartup(IConfiguration env, IEnumerable<Claim> claims) : base(env)
        {
            Claims = claims;
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseMiddleware<TestAuthMiddleware>(Claims);
        }
    }

    class TestAuthMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IEnumerable<Claim> claims;

        public TestAuthMiddleware(RequestDelegate rd)
        {
            next = rd;
        }

        public TestAuthMiddleware(RequestDelegate rd, IEnumerable<Claim> claims) : this(rd)
        {
            this.claims = claims;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var identity = new ClaimsIdentity("cookies");
            identity.AddClaim(new Claim("sub", "AEA56590-A116-4009-A2A7-CD5B17B0DB39"));

            if (claims != null)
            {
                identity.AddClaims(claims);
            }

            httpContext.User.AddIdentity(identity);

            await next.Invoke(httpContext);
        }
    }
}

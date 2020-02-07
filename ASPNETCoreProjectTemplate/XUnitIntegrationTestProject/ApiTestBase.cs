using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Claims;

namespace XUnitIntegrationTestProject
{
    public class ApiTestBase
    {
        public TestServer CreateServer()
        {
            var hostBuilder = GetWebHostBuilder();

            hostBuilder.UseStartup<ApiTestStartup>();

            return new TestServer(hostBuilder);
        }

        public TestServer CreateServer(IEnumerable<Claim> claims)
        {
            var hostBuilder = GetWebHostBuilder();

            hostBuilder
                .ConfigureServices(services =>
                {
                    if (claims != null)
                    {
                        services.AddSingleton(claims);
                    }
                })
                .UseStartup<ApiWithUserTestStartup>();

            return new TestServer(hostBuilder);
        }

        private IWebHostBuilder GetWebHostBuilder()
        {
            var path = Assembly.GetAssembly(typeof(ApiTestBase)).Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                });

            return hostBuilder;
        }
    }
}

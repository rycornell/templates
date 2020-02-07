using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace XUnitIntegrationTestProject
{
    public class WeatherForecastApiTest : ApiTestBase
    {
        [Fact]
        public async Task Test1()
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, "bb03ec14-b307-432b-be34-f2423f234c43"));
            var id = 1;

            using (var server = CreateServer(claims))
            {
                var response = await server.CreateClient()
                   .GetAsync(Routes.GetWeatherForecast(id));

                response.EnsureSuccessStatusCode();
            }
        }

        private static class Routes
        {
            private const string ApiUrlBase = "weatherforecast";

            public static string GetWeatherForecast(int id)
            {
                return $"{ApiUrlBase}/{id.ToString()}";
            }
        }
    }
}

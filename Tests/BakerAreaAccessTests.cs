using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Carlile_Cookie_Competition.Tests.Infrastructure;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Carlile_Cookie_Competition.Tests
{
    public class BakerAreaAccessTests : IClassFixture<CustomWebAppFactory<CookieVotingApi.Startup>>
    {
        private readonly CustomWebAppFactory<CookieVotingApi.Startup> _factory;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public BakerAreaAccessTests(CustomWebAppFactory<CookieVotingApi.Startup> factory) => _factory = factory;

        [Fact]
        public async Task BakerArea_ReturnsRedirect_WhenNotLoggedIn()
        {
            var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var resp = await client.GetAsync("/baker/area");
            // Controller redirects to baker-select when not authenticated
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Contains("/baker-select.html", resp.Headers.Location.OriginalString);
        }

        [Fact]
        public async Task BakerArea_Allows_Access_When_LoggedIn()
        {
            var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            // Create baker row
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<Carlile_Cookie_Competition.Data.AppDbContext>();
                db.Bakers.Add(new Carlile_Cookie_Competition.Models.Baker("AreaBaker"));
                db.SaveChanges();
            }

            // Register PIN via login endpoint to get cookie set
            var loginBody = new { bakerName = "AreaBaker", pin = "9999" };
            var content = new StringContent(JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json");
            var loginResp = await client.PostAsync("/api/bakers/login", content);
            Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);

            // Now request /baker/area (client carries cookies automatically)
            var areaResp = await client.GetAsync("/baker/area");
            // Should be OK (200) or redirect allowed; our controller returns File (200)
            Assert.Equal(HttpStatusCode.OK, areaResp.StatusCode);
            var body = await areaResp.Content.ReadAsStringAsync();
            Assert.Contains("Baker Area", body);
        }
    }
}

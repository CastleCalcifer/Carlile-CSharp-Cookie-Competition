using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Carlile_Cookie_Competition.Tests.Infrastructure;
using Xunit;

namespace Carlile_Cookie_Competition.Tests
{
    // Replace CookieVotingApi.Startup with your Startup type if namespace differs
    public class BakerLoginTests : IClassFixture<CustomWebAppFactory<CookieVotingApi.Startup>>
    {
        private readonly CustomWebAppFactory<CookieVotingApi.Startup> _factory;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public BakerLoginTests(CustomWebAppFactory<CookieVotingApi.Startup> factory) => _factory = factory;

        [Fact]
        public async Task Register_then_Login_Sets_Cookie()
        {
            var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            // Seed baker row (without PinHash) via /api/bakers? (We seed directly by calling a controller endpoint is easiest)
            // But simpler: call POST /api/bakers/login with a baker name that exists in seeder OR create baker via an admin endpoint.
            // For these tests we'll assume the DB is empty and we create the Baker by seeder endpoint or you can call DB directly.
            // For simplicity, create baker via POST to /api/admin/create-baker (if you have it), otherwise we will call the login endpoint
            // which requires baker row exist; so insert directly via context service:
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<Carlile_Cookie_Competition.Data.AppDbContext>();
                db.Bakers.Add(new Carlile_Cookie_Competition.Models.Baker("TestBaker"));
                db.SaveChanges();
            }

            var loginBody = new { bakerName = "TestBaker", pin = "1234" };
            var content = new StringContent(JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json");

            // First call: should register and set cookie
            var resp = await client.PostAsync("/api/bakers/login", content);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var respText = await resp.Content.ReadAsStringAsync();
            var payload = JsonSerializer.Deserialize<JsonElement>(respText, _jsonOptions);
            Assert.True(payload.GetProperty("success").GetBoolean());

            // Cookie should be set in client handler
            var cookieHeaders = resp.Headers.GetValues("Set-Cookie");
            Assert.NotNull(cookieHeaders);

            // Subsequent login with same PIN should succeed and also set cookie (or refresh)
            var content2 = new StringContent(JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json");
            var resp2 = await client.PostAsync("/api/bakers/login", content2);
            Assert.Equal(HttpStatusCode.OK, resp2.StatusCode);
            var resp2Text = await resp2.Content.ReadAsStringAsync();
            var payload2 = JsonSerializer.Deserialize<JsonElement>(resp2Text, _jsonOptions);
            Assert.True(payload2.GetProperty("success").GetBoolean());
        }
    }
}

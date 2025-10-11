using System;
using System.Collections.Generic;
using System.Linq;
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
    public class VotingTests : IClassFixture<CustomWebAppFactory<CookieVotingApi.Startup>>
    {
        private readonly CustomWebAppFactory<CookieVotingApi.Startup> _factory;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public VotingTests(CustomWebAppFactory<CookieVotingApi.Startup> factory) => _factory = factory;

        private async Task<int> SeedCookiesAndGetOneCookieId()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Carlile_Cookie_Competition.Data.AppDbContext>();

            // Create 3 cookies and a baker
            var c1 = new Carlile_Cookie_Competition.Models.Cookie("C1", 2024, "/images/c1.jpg", "Baker1");
            var c2 = new Carlile_Cookie_Competition.Models.Cookie("C2", 2024, "/images/c2.jpg", "Baker2");
            var c3 = new Carlile_Cookie_Competition.Models.Cookie("C3", 2024, "/images/c3.jpg", "Baker3");

            db.Cookies.AddRange(c1, c2, c3);
            var baker = new Carlile_Cookie_Competition.Models.Baker("VoterA", c1.Id);
            db.Bakers.Add(baker);
            db.SaveChanges();

            return baker.Id;
        }

        [Fact]
        public async Task Vote_Succeeds_And_Prevents_DoubleVote()
        {
            var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            // Seed DB
            var bakerId = await SeedCookiesAndGetOneCookieId();

            // Login to get cookie set
            var loginBody = new { bakerName = "VoterA", pin = "2468" };
            var content = new StringContent(JsonSerializer.Serialize(loginBody), Encoding.UTF8, "application/json");
            var loginResp = await client.PostAsync("/api/bakers/login", content);
            Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);

            // Prepare vote payload: assume there are 3 cookies to rank
            // Determine cookie ids from API
            var cookiesResp = await client.GetAsync($"/api/cookies?year=2024&excludeBakerId={bakerId}");
            Assert.Equal(HttpStatusCode.OK, cookiesResp.StatusCode);
            var cookiesText = await cookiesResp.Content.ReadAsStringAsync();
            var cookiePayload = JsonSerializer.Deserialize<JsonElement>(cookiesText, _jsonOptions);
            var cookieArray = cookiePayload.GetProperty("data");
            var cookieIds = cookieArray.EnumerateArray().Select(x => x.GetProperty("id").GetInt32()).ToList();

            // We expect cookieIds length >= 2 for a meaningful vote
            Assert.True(cookieIds.Count >= 1);

            // Build ranked list (top-to-bottom). The API earlier expected cookieIds ordered by rank indices (1..N)
            // For this test we submit cookieIds as returned order
            var voteReq = new { year = 2024, cookieIds = cookieIds };

            var voteContent = new StringContent(JsonSerializer.Serialize(voteReq), Encoding.UTF8, "application/json");
            var voteResp = await client.PostAsync("/api/voter/vote", voteContent);
            Assert.Equal(HttpStatusCode.OK, voteResp.StatusCode);

            // Now attempt to vote again -> should be rejected (Conflict 409)
            var voteResp2 = await client.PostAsync("/api/voter/vote", voteContent);
            Assert.Equal(HttpStatusCode.Conflict, voteResp2.StatusCode);

            // Check baker.HasVoted flag is true in DB
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Carlile_Cookie_Competition.Data.AppDbContext>();
            var baker = db.Bakers.First(b => b.BakerName == "VoterA");
            Assert.True(baker.HasVoted);
        }
    }
}

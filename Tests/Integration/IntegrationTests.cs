using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Models;
using Carlile_Cookie_Competition.Dtos;
using FluentAssertions;
using Xunit;
using Carlile_Cookie_Competition.Tests.Infrastructure;

namespace Carlile_Cookie_Competition.Tests.Integration
{
    public class IntegrationTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public IntegrationTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetBakers_ShouldReturnAllBakers()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/bakers");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var content = await response.Content.ReadFromJsonAsync<dynamic>();
            content.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCookies_ShouldReturnCookiesForYear()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/cookies?year=2024");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var content = await response.Content.ReadFromJsonAsync<dynamic>();
            content.Should().NotBeNull();
        }

        [Fact]
        public async Task SubmitVote_ShouldRecordVotes()
        {
            // Arrange
            await SeedTestData();
            var request = new SubmitVotesRequest(2024, new List<int> { 1, 2 }, "test-voter");

            // Act
            var response = await _client.PostAsJsonAsync("/api/voter/vote", request);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task SubmitAwards_ShouldRecordAwards()
        {
            // Arrange
            await SeedTestData();
            var request = new AwardSubmitRequest(2024, 1, 2, "test-voter");

            // Act
            var response = await _client.PostAsJsonAsync("/api/awards", request);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task GetResults_ShouldReturnRankedResults()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/results?year=2024");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var content = await response.Content.ReadFromJsonAsync<dynamic>();
            content.Should().NotBeNull();
        }

        [Fact]
        public async Task BakerLogin_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            await SeedTestData();
            var request = new LoginRequest { BakerName = "Christopher", Pin = "1234" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bakers/login", request);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task BakerLogin_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            await SeedTestData();
            var request = new LoginRequest { BakerName = "Christopher", Pin = "wrong-pin" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bakers/login", request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task BakerArea_WithoutAuthentication_ShouldRedirect()
        {
            // Act
            var response = await _client.GetAsync("/baker/area");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
        }

        private async Task SeedTestData()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Add test cookies
            var cookies = new List<Cookie>
            {
                new Cookie("Test Cookie 1", 2024, "image1.jpg", "Christopher") { Id = 1 },
                new Cookie("Test Cookie 2", 2024, "image2.jpg", "Bridget") { Id = 2 }
            };
            context.Cookies.AddRange(cookies);

            // Add test bakers
            var bakers = new List<Baker>
            {
                new Baker("Christopher", 1),
                new Baker("Bridget", 2)
            };
            context.Bakers.AddRange(bakers);

            await context.SaveChangesAsync();
        }

        public async Task InitializeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.EnsureDeletedAsync();
            _client.Dispose();
        }
    }
}

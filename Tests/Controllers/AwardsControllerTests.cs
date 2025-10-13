using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Carlile_Cookie_Competition.Controllers;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Models;
using Carlile_Cookie_Competition.Dtos;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Controllers
{
    public class AwardsControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly AwardsController _controller;

        public AwardsControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new AwardsController(_context);
        }

        [Fact]
        public async Task PostAwards_WithValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 },
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2") { Id = 2 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new AwardSubmitRequest(2024, 1, 2, "voter123");

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();

            // Verify creative and presentation points were incremented
            var updatedCookies = await _context.Cookies.ToListAsync();
            var creativeWinner = updatedCookies.First(c => c.Id == 1);
            var presentationWinner = updatedCookies.First(c => c.Id == 2);

            creativeWinner.CreativePoints.Should().Be(1);
            presentationWinner.PresentationPoints.Should().Be(1);
        }

        [Fact]
        public async Task PostAwards_WithNullRequest_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.PostAwards(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PostAwards_WithInvalidMostCreativeId_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new AwardSubmitRequest(2024, 0, 1, "voter123");

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PostAwards_WithInvalidBestPresentationId_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new AwardSubmitRequest(2024, 1, 0, "voter123");

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PostAwards_WithNegativeIds_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new AwardSubmitRequest(2024, -1, -2, "voter123");

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PostAwards_WithNonExistentCookies_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new AwardSubmitRequest(2024, 999, 998, "voter123");

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PostAwards_WithWrongYear_ShouldReturnBadRequest()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2023, "image1.jpg", "Baker1") { Id = 1 },
                new Cookie("Cookie2", 2023, "image2.jpg", "Baker2") { Id = 2 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new AwardSubmitRequest(2024, 1, 2, "voter123");

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PostAwards_WithSameCookieForBothAwards_ShouldReturnSuccess()
        {
            // Arrange
            var cookie = new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 };
            _context.Cookies.Add(cookie);
            await _context.SaveChangesAsync();

            var request = new AwardSubmitRequest(2024, 1, 1, "voter123");

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            // Verify both points were incremented
            var updatedCookie = await _context.Cookies.FindAsync(1);
            updatedCookie!.CreativePoints.Should().Be(1);
            updatedCookie.PresentationPoints.Should().Be(1);
        }

        [Fact]
        public async Task PostAwards_WithNullVoterId_ShouldReturnSuccess()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 },
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2") { Id = 2 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new AwardSubmitRequest(2024, 1, 2, null);

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PostAwards_WithEmptyVoterId_ShouldReturnSuccess()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 },
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2") { Id = 2 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new AwardSubmitRequest(2024, 1, 2, "");

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PostAwards_WithExistingVote_ShouldReturnConflict()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 },
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2") { Id = 2 }
            };
            _context.Cookies.AddRange(cookies);

            // Add existing vote with -1 points (award vote)
            var existingVote = new Vote
            {
                CookieId = 1,
                VoterId = "voter123",
                Points = -1
            };
            _context.Votes.Add(existingVote);
            await _context.SaveChangesAsync();

            var request = new AwardSubmitRequest(2024, 1, 2, "voter123");

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task PostAwards_ShouldIncrementExistingPoints()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1, CreativePoints = 5, PresentationPoints = 3 },
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2") { Id = 2, CreativePoints = 2, PresentationPoints = 7 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new AwardSubmitRequest(2024, 1, 2, "voter123");

            // Act
            var result = await _controller.PostAwards(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var updatedCookies = await _context.Cookies.ToListAsync();
            var creativeWinner = updatedCookies.First(c => c.Id == 1);
            var presentationWinner = updatedCookies.First(c => c.Id == 2);

            creativeWinner.CreativePoints.Should().Be(6); // 5 + 1
            presentationWinner.PresentationPoints.Should().Be(8); // 7 + 1
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

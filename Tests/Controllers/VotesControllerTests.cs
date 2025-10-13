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
    public class VotesControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly VoterController _controller;

        public VotesControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new VoterController(_context);
        }

        [Fact]
        public async Task SubmitVote_WithValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 },
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2") { Id = 2 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new SubmitVotesRequest(2024, new List<int> { 1, 2 }, "voter123");

            // Act
            var result = await _controller.SubmitVote(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();

            // Verify votes were recorded
            var votes = await _context.Votes.ToListAsync();
            votes.Should().HaveCount(2);
            votes[0].Points.Should().Be(4); // First place
            votes[1].Points.Should().Be(3); // Second place

            // Verify cookie scores were updated
            var updatedCookies = await _context.Cookies.ToListAsync();
            updatedCookies[0].Score.Should().Be(4);
            updatedCookies[1].Score.Should().Be(3);
        }

        [Fact]
        public async Task SubmitVote_WithNullRequest_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.SubmitVote(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SubmitVote_WithEmptyCookieIds_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new SubmitVotesRequest(2024, new List<int>(), "voter123");

            // Act
            var result = await _controller.SubmitVote(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SubmitVote_WithDuplicateCookieIds_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new SubmitVotesRequest(2024, new List<int> { 1, 1 }, "voter123");

            // Act
            var result = await _controller.SubmitVote(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SubmitVote_WithInvalidCookieIds_ShouldReturnBadRequest()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new SubmitVotesRequest(2024, new List<int> { 1, 999 }, "voter123");

            // Act
            var result = await _controller.SubmitVote(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SubmitVote_WithWrongYear_ShouldReturnBadRequest()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2023, "image1.jpg", "Baker1") { Id = 1 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new SubmitVotesRequest(2024, new List<int> { 1 }, "voter123");

            // Act
            var result = await _controller.SubmitVote(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SubmitVote_WithFourCookies_ShouldAssignCorrectPoints()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 },
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2") { Id = 2 },
                new Cookie("Cookie3", 2024, "image3.jpg", "Baker3") { Id = 3 },
                new Cookie("Cookie4", 2024, "image4.jpg", "Baker4") { Id = 4 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new SubmitVotesRequest(2024, new List<int> { 1, 2, 3, 4 }, "voter123");

            // Act
            var result = await _controller.SubmitVote(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var votes = await _context.Votes.OrderBy(v => v.CookieId).ToListAsync();
            votes.Should().HaveCount(4);
            votes[0].Points.Should().Be(4); // 1st place
            votes[1].Points.Should().Be(3); // 2nd place
            votes[2].Points.Should().Be(2); // 3rd place
            votes[3].Points.Should().Be(1); // 4th place
        }

        [Fact]
        public async Task SubmitVote_WithNullVoterId_ShouldStillRecordVotes()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new SubmitVotesRequest(2024, new List<int> { 1 }, null);

            // Act
            var result = await _controller.SubmitVote(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var votes = await _context.Votes.ToListAsync();
            votes.Should().HaveCount(1);
            votes[0].VoterId.Should().BeNull();
        }

        [Fact]
        public async Task SubmitVote_WithEmptyVoterId_ShouldStillRecordVotes()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            var request = new SubmitVotesRequest(2024, new List<int> { 1 }, "");

            // Act
            var result = await _controller.SubmitVote(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var votes = await _context.Votes.ToListAsync();
            votes.Should().HaveCount(1);
            votes[0].VoterId.Should().BeNull();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

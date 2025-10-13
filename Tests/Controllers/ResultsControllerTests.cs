using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Carlile_Cookie_Competition.Controllers;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Models;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Controllers
{
    public class ResultsControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ResultsController _controller;

        public ResultsControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new ResultsController(_context);
        }

        [Fact]
        public async Task Get_WithValidYear_ShouldReturnRankedResults()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1, Score = 10, CreativePoints = 5, PresentationPoints = 3 },
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2") { Id = 2, Score = 15, CreativePoints = 3, PresentationPoints = 7 },
                new Cookie("Cookie3", 2023, "image3.jpg", "Baker3") { Id = 3, Score = 20, CreativePoints = 8, PresentationPoints = 2 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Get(2024);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();

            var data = response.GetProperty("data");
            var ranked = data.GetProperty("ranked");
            ranked.GetArrayLength().Should().Be(2);

            // Should be ranked by score (highest first)
            ranked[0].GetProperty("score").GetInt32().Should().Be(15); // Cookie2
            ranked[1].GetProperty("score").GetInt32().Should().Be(10); // Cookie1
        }

        [Fact]
        public async Task Get_WithInvalidYear_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.Get(0);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Get_WithNegativeYear_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.Get(-1);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Get_WithNoCookies_ShouldReturnEmptyResults()
        {
            // Act
            var result = await _controller.Get(2024);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();

            var data = response.GetProperty("data");
            data.GetProperty("ranked").GetArrayLength().Should().Be(0);
            data.GetProperty("awards").GetProperty("presentation").ValueKind.Should().Be(JsonValueKind.Null);
            data.GetProperty("awards").GetProperty("creative").ValueKind.Should().Be(JsonValueKind.Null);
        }

        [Fact]
        public async Task Get_ShouldReturnCorrectAwardWinners()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1, Score = 10, CreativePoints = 8, PresentationPoints = 2 },
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2") { Id = 2, Score = 15, CreativePoints = 3, PresentationPoints = 7 }
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Get(2024);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));

            var data = response.GetProperty("data");
            var awards = data.GetProperty("awards");

            // Cookie1 should win creative award (8 points)
            var creativeWinner = awards.GetProperty("creative");
            creativeWinner.GetProperty("id").GetInt32().Should().Be(1);
            creativeWinner.GetProperty("creativePoints").GetInt32().Should().Be(8);

            // Cookie2 should win presentation award (7 points)
            var presentationWinner = awards.GetProperty("presentation");
            presentationWinner.GetProperty("id").GetInt32().Should().Be(2);
            presentationWinner.GetProperty("presentationPoints").GetInt32().Should().Be(7);
        }

        [Fact]
        public async Task Get_ShouldReturnCorrectCookieProperties()
        {
            // Arrange
            var cookie = new Cookie("Test Cookie", 2024, "test-image.jpg", "Test Baker")
            {
                Id = 1,
                Score = 20,
                CreativePoints = 10,
                PresentationPoints = 5
            };
            _context.Cookies.Add(cookie);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Get(2024);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));

            var data = response.GetProperty("data");
            var ranked = data.GetProperty("ranked");
            var cookieData = ranked[0];

            cookieData.GetProperty("id").GetInt32().Should().Be(1);
            cookieData.GetProperty("cookieName").GetString().Should().Be("Test Cookie");
            cookieData.GetProperty("score").GetInt32().Should().Be(20);
            cookieData.GetProperty("imageUrl").GetString().Should().Be("test-image.jpg");
            cookieData.GetProperty("creativePoints").GetInt32().Should().Be(10);
            cookieData.GetProperty("presentationPoints").GetInt32().Should().Be(5);
        }

        [Fact]
        public async Task Get_WithEmptyImage_ShouldUsePlaceholder()
        {
            // Arrange
            var cookie = new Cookie("Test Cookie", 2024, "", "Test Baker") { Id = 1 };
            _context.Cookies.Add(cookie);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Get(2024);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));

            var data = response.GetProperty("data");
            var ranked = data.GetProperty("ranked");
            var cookieData = ranked[0];

            cookieData.GetProperty("imageUrl").GetString().Should().Be("/images/placeholder.jpg");
        }

        [Fact]
        public async Task Get_WithWhitespaceImage_ShouldUsePlaceholder()
        {
            // Arrange
            var cookie = new Cookie("Test Cookie", 2024, "   ", "Test Baker") { Id = 1 };
            _context.Cookies.Add(cookie);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Get(2024);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));

            var data = response.GetProperty("data");
            var ranked = data.GetProperty("ranked");
            var cookieData = ranked[0];

            cookieData.GetProperty("imageUrl").GetString().Should().Be("/images/placeholder.jpg");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

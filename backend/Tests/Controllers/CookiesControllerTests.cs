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
    public class CookiesControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly CookiesController _controller;

        public CookiesControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new CookiesController(_context);
        }

        [Fact]
        public async Task Get_WithValidYear_ShouldReturnCookiesForYear()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1"),
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2"),
                new Cookie("Cookie3", 2023, "image3.jpg", "Baker3")
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
            response.GetProperty("data").GetArrayLength().Should().Be(2);
        }

        [Fact]
        public async Task Get_WithExcludeBakerId_ShouldExcludeBakerCookie()
        {
            // Arrange
            var baker = new Baker("TestBaker", 1);
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1") { Id = 1 },
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2") { Id = 2 }
            };
            _context.Bakers.Add(baker);
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Get(2024, 1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();
            response.GetProperty("data").GetArrayLength().Should().Be(1);
        }

        [Fact]
        public async Task Get_WithNonExistentBaker_ShouldReturnAllCookies()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2024, "image1.jpg", "Baker1"),
                new Cookie("Cookie2", 2024, "image2.jpg", "Baker2")
            };
            _context.Cookies.AddRange(cookies);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Get(2024, 999);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();
            response.GetProperty("data").GetArrayLength().Should().Be(2);
        }

        [Fact]
        public async Task Get_WithNoCookiesForYear_ShouldReturnEmptyArray()
        {
            // Arrange
            var cookies = new List<Cookie>
            {
                new Cookie("Cookie1", 2023, "image1.jpg", "Baker1")
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
            response.GetProperty("data").GetArrayLength().Should().Be(0);
        }

        [Fact]
        public async Task Get_ShouldReturnCorrectCookieProperties()
        {
            // Arrange
            var cookie = new Cookie("Test Cookie", 2024, "test-image.jpg", "Test Baker") { Id = 1 };
            _context.Cookies.Add(cookie);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Get(2024);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();
            
            var data = response.GetProperty("data")[0];
            data.GetProperty("id").GetInt32().Should().Be(1);
            data.GetProperty("cookieName").GetString().Should().Be("Test Cookie");
            data.GetProperty("imageUrl").GetString().Should().Be("test-image.jpg");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

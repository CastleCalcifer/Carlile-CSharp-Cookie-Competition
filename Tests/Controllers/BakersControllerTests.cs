using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Carlile_Cookie_Competition.Controllers;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Models;
using Carlile_Cookie_Competition.Dtos;
using Carlile_Cookie_Competition.Tests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Controllers
{
    public class BakersControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly BakersController _controller;
        private readonly Mock<IPasswordHasher<Baker>> _mockPasswordHasher;
        private readonly Mock<IDataProtectionProvider> _mockDataProtectionProvider;
        private readonly Mock<IDataProtector> _mockDataProtector;
        private readonly Mock<IDataProtectionService> _mockDataProtectionService;
        private readonly Mock<ILogger<BakersController>> _mockLogger;

        public BakersControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _mockPasswordHasher = new Mock<IPasswordHasher<Baker>>();
            _mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
            _mockDataProtector = new Mock<IDataProtector>();
            _mockDataProtectionService = new Mock<IDataProtectionService>();
            _mockLogger = new Mock<ILogger<BakersController>>();

            _mockDataProtectionProvider
                .Setup(x => x.CreateProtector(It.IsAny<string>()))
                .Returns(_mockDataProtector.Object);

            _controller = new BakersController(
                _context,
                _mockPasswordHasher.Object,
                _mockDataProtectionProvider.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllBakers()
        {
            // Arrange
            var bakers = new List<Baker>
            {
                new Baker("Baker1", 1),
                new Baker("Baker2", 2),
                new Baker("Baker3", null)
            };
            _context.Bakers.AddRange(bakers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();
            response.GetProperty("data").GetArrayLength().Should().Be(3);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var baker = new Baker("TestBaker", 1);
            baker.PinHash = "hashed_pin";
            _context.Bakers.Add(baker);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequest { BakerName = "TestBaker", Pin = "1234" };

            _mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(It.IsAny<Baker>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Success);

            _mockDataProtectionService
                .Setup(x => x.Protect(It.IsAny<string>()))
                .Returns("protected_value");

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var baker = new Baker("TestBaker", 1);
            baker.PinHash = "hashed_pin";
            _context.Bakers.Add(baker);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequest { BakerName = "TestBaker", Pin = "wrong_pin" };

            _mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(It.IsAny<Baker>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Failed);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_WithNonExistentBaker_ShouldReturnNotFound()
        {
            // Arrange
            var loginRequest = new LoginRequest { BakerName = "NonExistentBaker", Pin = "1234" };

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Login_WithNullRequest_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.Login(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_WithEmptyBakerName_ShouldReturnBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest { BakerName = "", Pin = "1234" };

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_WithEmptyPin_ShouldReturnBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest { BakerName = "TestBaker", Pin = "" };

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_WithNewBaker_ShouldRegisterPinAndReturnCreated()
        {
            // Arrange
            var baker = new Baker("NewBaker", 1);
            _context.Bakers.Add(baker);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequest { BakerName = "NewBaker", Pin = "1234" };

            _mockPasswordHasher
                .Setup(x => x.HashPassword(It.IsAny<Baker>(), It.IsAny<string>()))
                .Returns("hashed_pin");

            _mockDataProtectionService
                .Setup(x => x.Protect(It.IsAny<string>()))
                .Returns("protected_value");

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();
            response.GetProperty("created").GetBoolean().Should().BeTrue();

            // Verify pin was hashed and stored
            var updatedBaker = await _context.Bakers.FindAsync(baker.Id);
            updatedBaker!.PinHash.Should().Be("hashed_pin");
        }

        [Fact]
        public async Task Current_WithValidCookie_ShouldReturnBakerInfo()
        {
            // Arrange
            var baker = new Baker("TestBaker", 1);
            _context.Bakers.Add(baker);
            await _context.SaveChangesAsync();

            _mockDataProtectionService
                .Setup(x => x.Unprotect(It.IsAny<string>()))
                .Returns($"{baker.Id}|{DateTimeOffset.UtcNow.AddMinutes(60).ToUnixTimeSeconds()}");

            // Act
            var result = await _controller.Current();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();
            response.GetProperty("data").GetProperty("bakerName").GetString().Should().Be("TestBaker");
        }

        [Fact]
        public async Task Current_WithInvalidCookie_ShouldReturnNullData()
        {
            // Arrange
            _mockDataProtectionService
                .Setup(x => x.Unprotect(It.IsAny<string>()))
                .Throws(new Exception("Invalid cookie"));

            // Act
            var result = await _controller.Current();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(okResult!.Value));
            response.GetProperty("success").GetBoolean().Should().BeTrue();
            response.GetProperty("data").ValueKind.Should().Be(JsonValueKind.Null);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

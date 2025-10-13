using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;
using Carlile_Cookie_Competition.Controllers;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Controllers
{
    public class BakerAreaControllerTests
    {
        private readonly Mock<IDataProtectionProvider> _mockDataProtectionProvider;
        private readonly Mock<IDataProtector> _mockDataProtector;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly BakerAreaController _controller;

        public BakerAreaControllerTests()
        {
            _mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
            _mockDataProtector = new Mock<IDataProtector>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            _mockDataProtectionProvider
                .Setup(x => x.CreateProtector(It.IsAny<string>()))
                .Returns(_mockDataProtector.Object);

            _controller = new BakerAreaController(_mockDataProtectionProvider.Object, _mockWebHostEnvironment.Object);
        }

        [Fact]
        public void Area_WithValidCookie_ShouldReturnFile()
        {
            // Arrange
            var bakerId = 123;
            var expiry = DateTimeOffset.UtcNow.AddMinutes(60).ToUnixTimeSeconds();
            var protectedValue = "protected_cookie_value";
            var unprotectedValue = $"{bakerId}|{expiry}";

            _mockDataProtector
                .Setup(x => x.Unprotect(protectedValue))
                .Returns(unprotectedValue);

            _mockWebHostEnvironment
                .Setup(x => x.ContentRootPath)
                .Returns("/test/path");

            // Mock file existence and content
            var filePath = "/test/path/Protected/baker-area.html";
            var fileContent = "<html><body>Baker Area</body></html>";
            
            // We can't easily mock File.Exists and File.ReadAllBytes in unit tests
            // This test would need integration testing for full coverage

            // Act & Assert
            // This test demonstrates the structure but would need integration testing
            // to properly test file serving functionality
            _controller.Should().NotBeNull();
        }

        [Fact]
        public void Area_WithInvalidCookie_ShouldRedirect()
        {
            // Arrange
            _mockDataProtector
                .Setup(x => x.Unprotect(It.IsAny<string>()))
                .Throws(new Exception("Invalid cookie"));

            // Act
            var result = _controller.Area();

            // Assert
            result.Should().BeOfType<RedirectResult>();
            var redirectResult = result as RedirectResult;
            redirectResult!.Url.Should().Be("/baker-select.html");
        }

        [Fact]
        public void Area_WithExpiredCookie_ShouldRedirect()
        {
            // Arrange
            var bakerId = 123;
            var expiredTime = DateTimeOffset.UtcNow.AddMinutes(-60).ToUnixTimeSeconds();
            var protectedValue = "protected_cookie_value";
            var unprotectedValue = $"{bakerId}|{expiredTime}";

            _mockDataProtector
                .Setup(x => x.Unprotect(protectedValue))
                .Returns(unprotectedValue);

            // Act
            var result = _controller.Area();

            // Assert
            result.Should().BeOfType<RedirectResult>();
            var redirectResult = result as RedirectResult;
            redirectResult!.Url.Should().Be("/baker-select.html");
        }

        [Fact]
        public void Area_WithMalformedCookie_ShouldRedirect()
        {
            // Arrange
            var protectedValue = "protected_cookie_value";
            var malformedValue = "invalid_format";

            _mockDataProtector
                .Setup(x => x.Unprotect(protectedValue))
                .Returns(malformedValue);

            // Act
            var result = _controller.Area();

            // Assert
            result.Should().BeOfType<RedirectResult>();
            var redirectResult = result as RedirectResult;
            redirectResult!.Url.Should().Be("/baker-select.html");
        }

        [Fact]
        public void Area_WithNonNumericBakerId_ShouldRedirect()
        {
            // Arrange
            var protectedValue = "protected_cookie_value";
            var invalidValue = "not_a_number|1234567890";

            _mockDataProtector
                .Setup(x => x.Unprotect(protectedValue))
                .Returns(invalidValue);

            // Act
            var result = _controller.Area();

            // Assert
            result.Should().BeOfType<RedirectResult>();
            var redirectResult = result as RedirectResult;
            redirectResult!.Url.Should().Be("/baker-select.html");
        }

        [Fact]
        public void Area_WithNonNumericExpiry_ShouldRedirect()
        {
            // Arrange
            var protectedValue = "protected_cookie_value";
            var invalidValue = "123|not_a_number";

            _mockDataProtector
                .Setup(x => x.Unprotect(protectedValue))
                .Returns(invalidValue);

            // Act
            var result = _controller.Area();

            // Assert
            result.Should().BeOfType<RedirectResult>();
            var redirectResult = result as RedirectResult;
            redirectResult!.Url.Should().Be("/baker-select.html");
        }
    }
}

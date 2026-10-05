using Carlile_Cookie_Competition.Models;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Models
{
    public class CookieTests
    {
        [Fact]
        public void Constructor_WithAllParameters_ShouldSetProperties()
        {
            // Arrange
            var cookieName = "Test Cookie";
            var year = 2024;
            var image = "test-image.jpg";
            var bakerName = "Test Baker";

            // Act
            var cookie = new Cookie(cookieName, year, image, bakerName);

            // Assert
            cookie.CookieName.Should().Be(cookieName);
            cookie.Year.Should().Be(year);
            cookie.Image.Should().Be(image);
            cookie.BakerName.Should().Be(bakerName);
            cookie.Score.Should().Be(0);
            cookie.CreativePoints.Should().Be(0);
            cookie.PresentationPoints.Should().Be(0);
        }

        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            // Act
            var cookie = new Cookie();

            // Assert
            cookie.CookieName.Should().Be("");
            cookie.Year.Should().Be(0);
            cookie.Image.Should().Be("");
            cookie.BakerName.Should().Be("");
            cookie.Score.Should().Be(0);
            cookie.CreativePoints.Should().Be(0);
            cookie.PresentationPoints.Should().Be(0);
        }

        [Theory]
        [InlineData("Chocolate Chip")]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("Very Long Cookie Name With Special Characters !@#$%")]
        public void CookieName_ShouldAcceptVariousValues(string cookieName)
        {
            // Act
            var cookie = new Cookie(cookieName, 2024, "test.jpg", "Baker");

            // Assert
            cookie.CookieName.Should().Be(cookieName);
        }

        [Theory]
        [InlineData(2020)]
        [InlineData(2024)]
        [InlineData(2030)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void Year_ShouldAcceptVariousValues(int year)
        {
            // Act
            var cookie = new Cookie("Test Cookie", year, "test.jpg", "Baker");

            // Assert
            cookie.Year.Should().Be(year);
        }

        [Theory]
        [InlineData("image.jpg")]
        [InlineData("/images/cookie.jpg")]
        [InlineData("")]
        [InlineData("path/with/slashes/image.png")]
        public void Image_ShouldAcceptVariousValues(string image)
        {
            // Act
            var cookie = new Cookie("Test Cookie", 2024, image, "Baker");

            // Assert
            cookie.Image.Should().Be(image);
        }

        [Theory]
        [InlineData("Baker Name")]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("Very Long Baker Name")]
        public void BakerName_ShouldAcceptVariousValues(string bakerName)
        {
            // Act
            var cookie = new Cookie("Test Cookie", 2024, "test.jpg", bakerName);

            // Assert
            cookie.BakerName.Should().Be(bakerName);
        }
    }
}

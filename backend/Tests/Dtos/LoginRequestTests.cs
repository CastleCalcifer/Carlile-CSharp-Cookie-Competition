using Carlile_Cookie_Competition.Dtos;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Dtos
{
    public class LoginRequestTests
    {
        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            // Act
            var request = new LoginRequest();

            // Assert
            request.BakerName.Should().Be("");
            request.Pin.Should().Be("");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Test Baker")]
        [InlineData("A")]
        [InlineData("Very Long Baker Name")]
        public void BakerName_ShouldAcceptVariousValues(string bakerName)
        {
            // Arrange
            var request = new LoginRequest();

            // Act
            request.BakerName = bakerName;

            // Assert
            request.BakerName.Should().Be(bakerName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("1234")]
        [InlineData("A")]
        [InlineData("Very Long Pin String")]
        [InlineData("!@#$%^&*()")]
        public void Pin_ShouldAcceptVariousValues(string pin)
        {
            // Arrange
            var request = new LoginRequest();

            // Act
            request.Pin = pin;

            // Assert
            request.Pin.Should().Be(pin);
        }

        [Fact]
        public void Properties_ShouldBeSettable()
        {
            // Arrange
            var request = new LoginRequest();
            var bakerName = "Test Baker";
            var pin = "1234";

            // Act
            request.BakerName = bakerName;
            request.Pin = pin;

            // Assert
            request.BakerName.Should().Be(bakerName);
            request.Pin.Should().Be(pin);
        }
    }
}

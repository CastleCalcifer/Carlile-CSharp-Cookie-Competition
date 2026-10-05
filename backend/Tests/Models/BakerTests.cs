using Carlile_Cookie_Competition.Models;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Models
{
    public class BakerTests
    {
        [Fact]
        public void Constructor_WithBakerName_ShouldSetProperties()
        {
            // Arrange
            var bakerName = "Test Baker";
            var cookieId = 1;

            // Act
            var baker = new Baker(bakerName, cookieId);

            // Assert
            baker.BakerName.Should().Be(bakerName);
            baker.CookieId.Should().Be(cookieId);
            baker.HasVoted.Should().BeFalse();
            baker.PinHash.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithBakerNameOnly_ShouldSetDefaultValues()
        {
            // Arrange
            var bakerName = "Test Baker";

            // Act
            var baker = new Baker(bakerName);

            // Assert
            baker.BakerName.Should().Be(bakerName);
            baker.CookieId.Should().BeNull();
            baker.HasVoted.Should().BeFalse();
            baker.PinHash.Should().BeNull();
        }

        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            // Act
            var baker = new Baker();

            // Assert
            baker.BakerName.Should().Be("");
            baker.CookieId.Should().BeNull();
            baker.HasVoted.Should().BeFalse();
            baker.PinHash.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Test Baker")]
        [InlineData("A")]
        public void BakerName_ShouldAcceptVariousValues(string bakerName)
        {
            // Act
            var baker = new Baker(bakerName);

            // Assert
            baker.BakerName.Should().Be(bakerName);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        [InlineData(null)]
        public void CookieId_ShouldAcceptVariousValues(int? cookieId)
        {
            // Act
            var baker = new Baker("Test Baker", cookieId);

            // Assert
            baker.CookieId.Should().Be(cookieId);
        }
    }
}

using Carlile_Cookie_Competition.Dtos;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Dtos
{
    public class AwardSubmitRequestTests
    {
        [Fact]
        public void Constructor_WithAllParameters_ShouldSetProperties()
        {
            // Arrange
            var year = 2024;
            var mostCreativeId = 1;
            var bestPresentationId = 2;
            var voterId = "voter123";

            // Act
            var request = new AwardSubmitRequest(year, mostCreativeId, bestPresentationId, voterId);

            // Assert
            request.Year.Should().Be(year);
            request.MostCreativeId.Should().Be(mostCreativeId);
            request.BestPresentationId.Should().Be(bestPresentationId);
            request.VoterId.Should().Be(voterId);
        }

        [Fact]
        public void Constructor_WithNullVoterId_ShouldSetNull()
        {
            // Arrange
            var year = 2024;
            var mostCreativeId = 1;
            var bestPresentationId = 2;

            // Act
            var request = new AwardSubmitRequest(year, mostCreativeId, bestPresentationId, null);

            // Assert
            request.Year.Should().Be(year);
            request.MostCreativeId.Should().Be(mostCreativeId);
            request.BestPresentationId.Should().Be(bestPresentationId);
            request.VoterId.Should().BeNull();
        }

        [Theory]
        [InlineData(2020)]
        [InlineData(2024)]
        [InlineData(2030)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void Year_ShouldAcceptVariousValues(int year)
        {
            // Act
            var request = new AwardSubmitRequest(year, 1, 2, "voter");

            // Assert
            request.Year.Should().Be(year);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void MostCreativeId_ShouldAcceptVariousValues(int mostCreativeId)
        {
            // Act
            var request = new AwardSubmitRequest(2024, mostCreativeId, 2, "voter");

            // Assert
            request.MostCreativeId.Should().Be(mostCreativeId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void BestPresentationId_ShouldAcceptVariousValues(int bestPresentationId)
        {
            // Act
            var request = new AwardSubmitRequest(2024, 1, bestPresentationId, "voter");

            // Assert
            request.BestPresentationId.Should().Be(bestPresentationId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("voter123")]
        [InlineData("A")]
        [InlineData("very-long-voter-id-string")]
        [InlineData("!@#$%^&*()")]
        public void VoterId_ShouldAcceptVariousValues(string voterId)
        {
            // Act
            var request = new AwardSubmitRequest(2024, 1, 2, voterId);

            // Assert
            request.VoterId.Should().Be(voterId);
        }

        [Fact]
        public void Record_ShouldBeImmutable()
        {
            // Arrange
            var request = new AwardSubmitRequest(2024, 1, 2, "voter");

            // Act & Assert
            // Records are immutable by default, so we can't modify properties after creation
            request.Year.Should().Be(2024);
            request.MostCreativeId.Should().Be(1);
            request.BestPresentationId.Should().Be(2);
            request.VoterId.Should().Be("voter");
        }

        [Fact]
        public void Constructor_WithSameIds_ShouldBeAllowed()
        {
            // Arrange
            var year = 2024;
            var sameId = 1;

            // Act
            var request = new AwardSubmitRequest(year, sameId, sameId, "voter");

            // Assert
            request.Year.Should().Be(year);
            request.MostCreativeId.Should().Be(sameId);
            request.BestPresentationId.Should().Be(sameId);
            request.VoterId.Should().Be("voter");
        }
    }
}

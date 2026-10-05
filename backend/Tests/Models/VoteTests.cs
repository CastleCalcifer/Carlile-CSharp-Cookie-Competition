using Carlile_Cookie_Competition.Models;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Models
{
    public class VoteTests
    {
        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            // Act
            var vote = new Vote();

            // Assert
            vote.Id.Should().Be(0);
            vote.CookieId.Should().Be(0);
            vote.Cookie.Should().BeNull();
            vote.VoterId.Should().BeNull();
            vote.Points.Should().Be(0);
            vote.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public void CookieId_ShouldAcceptVariousValues(int cookieId)
        {
            // Arrange
            var vote = new Vote();

            // Act
            vote.CookieId = cookieId;

            // Assert
            vote.CookieId.Should().Be(cookieId);
        }

        [Theory]
        [InlineData("voter123")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("very-long-voter-id-string")]
        public void VoterId_ShouldAcceptVariousValues(string? voterId)
        {
            // Arrange
            var vote = new Vote();

            // Act
            vote.VoterId = voterId;

            // Assert
            vote.VoterId.Should().Be(voterId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void Points_ShouldAcceptVariousValues(int points)
        {
            // Arrange
            var vote = new Vote();

            // Act
            vote.Points = points;

            // Assert
            vote.Points.Should().Be(points);
        }

        [Fact]
        public void CreatedAt_ShouldBeSetToCurrentUtcTime()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var vote = new Vote();

            // Assert
            var afterCreation = DateTime.UtcNow;
            vote.CreatedAt.Should().BeOnOrAfter(beforeCreation);
            vote.CreatedAt.Should().BeOnOrBefore(afterCreation);
        }
    }
}

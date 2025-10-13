using Carlile_Cookie_Competition.Dtos;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Dtos
{
    public class SubmitVotesRequestTests
    {
        [Fact]
        public void Constructor_WithAllParameters_ShouldSetProperties()
        {
            // Arrange
            var year = 2024;
            var cookieIds = new List<int> { 1, 2, 3 };
            var voterId = "voter123";

            // Act
            var request = new SubmitVotesRequest(year, cookieIds, voterId);

            // Assert
            request.Year.Should().Be(year);
            request.CookieIds.Should().BeEquivalentTo(cookieIds);
            request.VoterId.Should().Be(voterId);
        }

        [Fact]
        public void Constructor_WithNullVoterId_ShouldSetNull()
        {
            // Arrange
            var year = 2024;
            var cookieIds = new List<int> { 1, 2, 3 };

            // Act
            var request = new SubmitVotesRequest(year, cookieIds, null);

            // Assert
            request.Year.Should().Be(year);
            request.CookieIds.Should().BeEquivalentTo(cookieIds);
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
            // Arrange
            var cookieIds = new List<int> { 1 };

            // Act
            var request = new SubmitVotesRequest(year, cookieIds, "voter");

            // Assert
            request.Year.Should().Be(year);
        }

        [Theory]
        [InlineData(new int[] { })]
        [InlineData(new int[] { 1 })]
        [InlineData(new int[] { 1, 2, 3, 4 })]
        [InlineData(new int[] { int.MaxValue, int.MinValue })]
        public void CookieIds_ShouldAcceptVariousValues(int[] cookieIds)
        {
            // Arrange
            var cookieIdsList = cookieIds.ToList();

            // Act
            var request = new SubmitVotesRequest(2024, cookieIdsList, "voter");

            // Assert
            request.CookieIds.Should().BeEquivalentTo(cookieIdsList);
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
            // Arrange
            var cookieIds = new List<int> { 1 };

            // Act
            var request = new SubmitVotesRequest(2024, cookieIds, voterId);

            // Assert
            request.VoterId.Should().Be(voterId);
        }

        [Fact]
        public void Record_ShouldBeImmutable()
        {
            // Arrange
            var originalCookieIds = new List<int> { 1, 2, 3 };
            var request = new SubmitVotesRequest(2024, originalCookieIds, "voter");

            // Act
            originalCookieIds.Add(4);

            // Assert
            request.CookieIds.Should().HaveCount(3);
            request.CookieIds.Should().NotContain(4);
        }
    }
}

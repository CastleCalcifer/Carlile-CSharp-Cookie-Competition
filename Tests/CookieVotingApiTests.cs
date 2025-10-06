//using Xunit;
//using FluentAssertions;
//using System.Threading.Tasks;
//using Carlile_Cookie_Competition.Tests;
////using Carlile_Cookie_Competition.Models;
////using Carlile_Cookie_Competition.Services;

//public class CookieVotingApiTests
//{
//    [Fact]
//    public async Task GetCookies_ReturnsAllCookies()
//    {
//        // Arrange
//        var service = TestHelper.CreateCookieServiceWithSampleData();

//        // Act
//        var cookies = await service.GetCookiesAsync();

//        // Assert
//        cookies.Should().NotBeNullOrEmpty();
//        cookies.Should().Contain(c => c.Name == "Chocolate Chip");
//    }

//    [Fact]
//    public async Task VoteCookie_AddsVoteForCookie()
//    {
//        // Arrange
//        var service = TestHelper.CreateCookieServiceWithSampleData();
//        var userId = "user1";
//        var cookieId = 1;

//        // Act
//        var result = await service.VoteAsync(userId, cookieId);

//        // Assert
//        result.Should().BeTrue();
//        var votes = await service.GetVotesForCookieAsync(cookieId);
//        votes.Should().Contain(v => v.UserId == userId);
//    }

//    [Fact]
//    public async Task GetResults_ReturnsVoteCountsForEachCookie()
//    {
//        // Arrange
//        var service = TestHelper.CreateCookieServiceWithSampleData();
//        await service.VoteAsync("user1", 1);
//        await service.VoteAsync("user2", 2);

//        // Act
//        var results = await service.GetResultsAsync();

//        // Assert
//        results.Should().Contain(r => r.CookieId == 1 && r.VoteCount == 1);
//        results.Should().Contain(r => r.CookieId == 2 && r.VoteCount == 1);
//    }

//    [Fact]
//    public async Task BakerCannotVoteForOwnCookie()
//    {
//        // Arrange
//        var service = TestHelper.CreateCookieServiceWithSampleData();
//        var bakerId = "baker1";
//        var ownCookieId = 1; // Assume baker1 owns cookie 1

//        // Act
//        var result = await service.BakerVoteAsync(bakerId, ownCookieId);

//        // Assert
//        result.Should().BeFalse();
//    }

//    [Fact]
//    public async Task BakerCanVoteForOtherCookies()
//    {
//        // Arrange
//        var service = TestHelper.CreateCookieServiceWithSampleData();
//        var bakerId = "baker1";
//        var otherCookieId = 2; // Assume baker1 does not own cookie 2

//        // Act
//        var result = await service.BakerVoteAsync(bakerId, otherCookieId);

//        // Assert
//        result.Should().BeTrue();
//    }

//    [Fact]
//    public async Task UserCannotVoteTwiceForSameCookie()
//    {
//        // Arrange
//        var service = TestHelper.CreateCookieServiceWithSampleData();
//        var userId = "user1";
//        var cookieId = 1;

//        // Act
//        var firstVote = await service.VoteAsync(userId, cookieId);
//        var secondVote = await service.VoteAsync(userId, cookieId);

//        // Assert
//        firstVote.Should().BeTrue();
//        secondVote.Should().BeFalse();
//    }
//}
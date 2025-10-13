// tests/CookieVotingApi.Tests/VoterControllerTests.cs
using Carlile_Cookie_Competition.Controllers;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Dtos;
using Carlile_Cookie_Competition.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Carlile_Cookie_Competition.Tests
{
    public class VoterControllerTests
    {
        private AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task SubmitVote_InsertsVotesAndIncrementsScores()
        {
            // arrange
            var db = CreateContext("SubmitVoteTest1");
            // seed 4 cookies for 2024
            db.Cookies.AddRange(new[]
            {
                new Cookie("A", 2024, "/images/a.jpg", "bakerA"),
                new Cookie("B", 2024, "/images/b.jpg", "bakerB"),
                new Cookie("C", 2024, "/images/c.jpg", "bakerC"),
                new Cookie("D", 2024, "/images/d.jpg", "bakerD"),
            });
            await db.SaveChangesAsync();

            var controller = new VoterController(db);

            var req = new SubmitVotesRequest(2024, new List<int> {
                db.Cookies.OrderBy(c => c.Id).Select(c => c.Id).First(),
                db.Cookies.OrderBy(c => c.Id).Select(c => c.Id).Skip(1).First(),
                db.Cookies.OrderBy(c => c.Id).Select(c => c.Id).Skip(2).First(),
                db.Cookies.OrderBy(c => c.Id).Select(c => c.Id).Skip(3).First(),
            }, "tester-1");

            // act
            var result = await controller.SubmitVote(req) as OkObjectResult;

            // assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            // Check votes inserted
            var votes = db.Votes.ToList();
            Assert.Equal(4, votes.Count);

            // Check scores - using PointsByRank mapping from controller: 10,7,4,1
            var cookieList = db.Cookies.OrderBy(c => c.Id).ToList();
            Assert.Equal(10, cookieList[0].Score);
            Assert.Equal(7, cookieList[1].Score);
            Assert.Equal(4, cookieList[2].Score);
            Assert.Equal(1, cookieList[3].Score);
        }

        [Fact]
        public async Task SubmitVote_PreventsDuplicateVoterIfVoterIdProvided()
        {
            var db = CreateContext("SubmitVoteTest2");
            db.Cookies.AddRange(new[]
            {
                new Cookie("A", 2024, "/i/a.jpg", "bA"),
                new Cookie("B", 2024, "/i/b.jpg", "bB"),
                new Cookie("C", 2024, "/i/c.jpg", "bC"),
                new Cookie("D", 2024, "/i/d.jpg", "bD"),
            });
            await db.SaveChangesAsync();

            var controller = new VoterController(db);

            var cookieIds = db.Cookies.OrderBy(c => c.Id).Select(c => c.Id).ToList();

            var req1 = new SubmitVotesRequest(2024, cookieIds, "voterX");
            var ok1 = await controller.SubmitVote(req1) as OkObjectResult;
            Assert.NotNull(ok1);

            // second attempt same voter => Conflict
            var req2 = new SubmitVotesRequest(2024, cookieIds, "voterX");
            var conflict = await controller.SubmitVote(req2) as ObjectResult;
            Assert.NotNull(conflict);
            Assert.Equal(409, conflict.StatusCode);
        }
    }
}

// Controllers/VoterController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Models;
using Carlile_Cookie_Competition.Dtos;

namespace Carlile_Cookie_Competition.Controllers
{
    /// <summary>
    /// API controller for submitting cookie ranking votes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VoterController : ControllerBase
    {
        private readonly AppDbContext _db;
        private static readonly int[] PointsByRank = new[] { 4, 3, 2, 1 };

        public VoterController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns the points for a given rank index.
        /// </summary>
        private static int PointsForRank(int rankIndex) =>
            rankIndex < PointsByRank.Length ? PointsByRank[rankIndex] : 0;

        /// <summary>
        /// Submits a ranked vote for cookies.
        /// </summary>
        /// <param name="req">Vote submission request.</param>
        /// <returns>Result of the submission.</returns>
        [HttpPost("vote")]
        public async Task<IActionResult> SubmitVote([FromBody] SubmitVotesRequest req)
        {
            if (req == null) return BadRequest(new { success = false, message = "Request body missing." });

            if (req.CookieIds == null || req.CookieIds.Count == 0)
                return BadRequest(new { success = false, message = "No cookieIds provided." });

            if (req.CookieIds.Distinct().Count() != req.CookieIds.Count)
                return BadRequest(new { success = false, message = "Duplicate cookieIds in ranking." });

            // Validate that all cookie IDs exist for the given year
            var cookieIds = req.CookieIds;
            var cookies = await _db.Cookies
                .Where(c => cookieIds.Contains(c.Id) && c.Year == req.Year)
                .ToListAsync();

            if (cookies.Count != cookieIds.Count)
                return BadRequest(new { success = false, message = "One or more cookieIds are invalid for the given year." });

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var votesToAdd = new List<Vote>();
                for (int rank = 0; rank < cookieIds.Count; rank++)
                {
                    var cid = cookieIds[rank];
                    var pts = PointsForRank(rank);

                    votesToAdd.Add(new Vote
                    {
                        CookieId = cid,
                        VoterId = string.IsNullOrWhiteSpace(req.VoterId) ? null : req.VoterId,
                        Points = pts,
                    });

                    // Atomically increment cookie score in the database
                    await _db.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE Cookie SET score = COALESCE(score, 0) + {pts} WHERE id = {cid}");
                }

                _db.Votes.AddRange(votesToAdd);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();
                return Ok(new { success = true, message = "Votes recorded." });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                Console.WriteLine($"[VoterController] Error recording votes: {ex}");
                return StatusCode(500, new { success = false, message = "Server error recording votes." });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Dtos;
using Carlile_Cookie_Competition.Models;

namespace Carlile_Cookie_Competition.Controllers
{
    /// <summary>
    /// API controller for submitting awards votes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AwardsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AwardsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Submits awards for Most Creative and Best Presentation cookies.
        /// </summary>
        /// <param name="req">Award submission request.</param>
        /// <returns>Result of the submission.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAwards([FromBody] AwardSubmitRequest req)
        {
            if (req == null) return BadRequest(new { success = false, message = "Request body is missing" });

            if (req.MostCreativeId <= 0 || req.BestPresentationId <= 0)
                return BadRequest(new { success = false, message = "Both awards must be selected" });

            // Validate cookies exist for the given year
            var ids = new[] { req.MostCreativeId, req.BestPresentationId };
            var cookies = await _db.Cookies
                .Where(c => ids.Contains(c.Id) && c.Year == req.Year)
                .ToListAsync();

            if (cookies.Count != 2)
                return BadRequest(new { success = false, message = "One or both cookie IDs are invalid for the selected year." });

            // Prevent duplicate award voting by voter id (if VoterId is provided)
            if (!string.IsNullOrWhiteSpace(req.VoterId))
            {
                var already = await _db.Votes
                    .Include(v => v.Cookie)
                    .AnyAsync(v => v.VoterId == req.VoterId && v.Cookie != null && v.Cookie.Year == req.Year && v.Points == -1);
                if (already)
                    return Conflict(new { success = false, message = "You have already submitted awards for this year." });
            }

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Increment creative and presentation points atomically
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE Cookie SET creative_points = COALESCE(creative_points, 0) + 1 WHERE id = {req.MostCreativeId}");
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE Cookie SET presentation_points = COALESCE(presentation_points, 0) + 1 WHERE id = {req.BestPresentationId}");

                // Optionally record the award submission for auditing
                // _db.Votes.Add(new Vote { CookieId = req.MostCreativeId, VoterId = req.VoterId, Points = -1 });
                // _db.Votes.Add(new Vote { CookieId = req.BestPresentationId, VoterId = req.VoterId, Points = -1 });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new { success = true, message = "Awards recorded." });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                Console.WriteLine($"[AwardsController] Error: {ex}");
                return StatusCode(500, new { success = false, message = "Server error recording awards." });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Dtos;
using Carlile_Cookie_Competition.Models;

namespace Carlile_Cookie_Competition.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwardsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AwardsController(AppDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> PostAwards([FromBody] AwardSubmitRequest req)
        {
            if (req == null) return BadRequest(new { success = false, message = "Request body is missing" });

            if (req.MostCreativeId <= 0 || req.BestPresentationId <= 0)
                return BadRequest(new { success = false, message = "Both awards must be selected" });

            if (req.MostCreativeId == req.BestPresentationId)
            {
                // optional: allow or reject; here we allow but you could BadRequest
            }

            // Validate cookies exist for year
            var ids = new[] { req.MostCreativeId, req.BestPresentationId };
            var cookies = await _db.Cookies
                .Where(c => ids.Contains(c.Id) && c.Year == req.Year)
                .ToListAsync();

            if (cookies.Count != 2)
                return BadRequest(new { success = false, message = "One or both cookie IDs are invalid for the selected year." });

            // Optional: prevent duplicate voting by voter id
            if (!string.IsNullOrWhiteSpace(req.VoterId))
            {
                var already = await _db.Votes
                    .Include(v => v.Cookie)
                    .AnyAsync(v => v.VoterId == req.VoterId && v.Cookie != null && v.Cookie.Year == req.Year && v.Points == -1);
                // NOTE: above uses a special Points == -1 sentinel for award votes if you choose to record them in Votes table,
                // or check a separate Awards table if you implement that. If not recording votes, omit this check.
                if (already)
                    return Conflict(new { success = false, message = "You have already submitted awards for this year." });
            }

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Atomic DB-side increments to avoid lost updates
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE Cookie SET creative_points = COALESCE(creative_points, 0) + 1 WHERE id = {req.MostCreativeId}");
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE Cookie SET presentation_points = COALESCE(presentation_points, 0) + 1 WHERE id = {req.BestPresentationId}");

                // Optionally record the award submission as a Vote or Award record:
                // Uncomment and adapt if you have an Award or Vote model for audit:
                /*
                _db.Votes.Add(new Vote { CookieId = req.MostCreativeId, VoterId = req.VoterId, Points = -1 });
                _db.Votes.Add(new Vote { CookieId = req.BestPresentationId, VoterId = req.VoterId, Points = -1 });
                await _db.SaveChangesAsync();
                */

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

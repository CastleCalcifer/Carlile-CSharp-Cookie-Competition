using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Models;

namespace Carlile_Cookie_Competition.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ResultsController(AppDbContext db)
        {
            _db = db;
        }

        // GET /api/results?year=2024
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int year)
        {
            if (year <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid year." });
            }

            var cookies = await _db.Cookies
                .Where(c => c.Year == year)
                .ToListAsync();

            if (cookies.Count == 0)
            {
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        ranked = Array.Empty<object>(),
                        awards = new { presentation = (object?)null, creative = (object?)null }
                    }
                });
            }

            // Rank cookies by total score (highest first)
            var ranked = cookies
                .OrderByDescending(c => c.Score)
                .Select(c => new
                {
                    id = c.Id,
                    cookieName = c.CookieName,
                    score = c.Score,
                    imageUrl = string.IsNullOrWhiteSpace(c.Image) ? "/images/placeholder.jpg" : c.Image,
                    creativePoints = c.CreativePoints,
                    presentationPoints = c.PresentationPoints
                })
                .ToList();

            // Determine award winners (highest creative & presentation points)
            var presentationWinner = cookies
                .OrderByDescending(c => c.PresentationPoints)
                .FirstOrDefault();

            var creativeWinner = cookies
                .OrderByDescending(c => c.CreativePoints)
                .FirstOrDefault();

            var result = new
            {
                ranked,
                awards = new
                {
                    presentation = presentationWinner == null ? null : new
                    {
                        id = presentationWinner.Id,
                        cookieName = presentationWinner.CookieName,
                        imageUrl = string.IsNullOrWhiteSpace(presentationWinner.Image) ? "/images/placeholder.jpg" : presentationWinner.Image,
                        presentationPoints = presentationWinner.PresentationPoints,
                        creativePoints = presentationWinner.CreativePoints,
                        score = presentationWinner.Score
                    },
                    creative = creativeWinner == null ? null : new
                    {
                        id = creativeWinner.Id,
                        cookieName = creativeWinner.CookieName,
                        imageUrl = string.IsNullOrWhiteSpace(creativeWinner.Image) ? "/images/placeholder.jpg" : creativeWinner.Image,
                        creativePoints = creativeWinner.CreativePoints,
                        presentationPoints = creativeWinner.PresentationPoints,
                        score = creativeWinner.Score
                    }
                }
            };

            return Ok(new { success = true, data = result });
        }
    }
}

using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Dtos;
using Carlile_Cookie_Competition.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class CookiesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CookiesController(AppDbContext db) => _db = db;

    // GET /api/cookies?year=2024
    // in CookiesController.cs GET action
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int year, [FromQuery] int? excludeBakerId = null)
    {
        var query = _db.Cookies.Where(c => c.Year == year);
        if (excludeBakerId.HasValue)
        {
            // find cookie id for baker, if any
            var baker = await _db.Bakers.FindAsync(excludeBakerId.Value);
            if (baker?.CookieId != null)
            {
                var excludeCookieId = baker.CookieId.Value;
                query = query.Where(c => c.Id != excludeCookieId);
            }
        }

        var cookies = await query.Select(c => new { id = c.Id, cookieName = c.CookieName, imageUrl = c.Image }).ToListAsync();
        return Ok(new { success = true, data = cookies });
    }

}
//    // POST /api/cookies
//    [HttpPost]
//    public async Task<IActionResult> Post([FromBody] VoteRequest data)
//    {
//        // data comes from JSON body
//        var cookie = await _db.Cookies.FindAsync(data.CookieId);
//        if (cookie == null) return NotFound();

//        cookie.Score += data.Points;
//        await _db.SaveChangesAsync();
//        return Ok(new { success = true });
//    }
//}

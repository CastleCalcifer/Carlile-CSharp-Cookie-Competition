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
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int year)
    {
        var cookies = await _db.Cookies
            .Where(c => c.Year == year)
            .OrderBy(c => c.Id)
            .Select(c => new { c.Id, c.CookieName, c.Image })
            .ToListAsync();

        return Ok(new { success = true, data = cookies });// automatically serialized to JSON
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

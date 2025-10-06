using Microsoft.AspNetCore.Mvc;
using Carlile_Cookie_Competition.Models;
using System.Linq;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Dtos;

[ApiController]
[Route("api/[controller]")]
public class CookiesController : ControllerBase
{
    private readonly AppDbContext _context;
    public CookiesController(AppDbContext context) => _context = context;

    [HttpGet]
    public ActionResult<IEnumerable<CookieDto>> GetCookies()
    {
        var cookies = _context.Cookies.Select(c => new CookieDto
        {
            Id = c.Id,
            Name = c.Name,
            BakerId = c.BakerId
        }).ToList();
        return Ok(cookies);
    }
}
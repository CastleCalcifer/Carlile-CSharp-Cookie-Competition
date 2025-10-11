using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Models;
using Carlile_Cookie_Competition.Dtos;
using System.Text;

namespace Carlile_Cookie_Competition.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BakersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<Baker> _hasher;
        private readonly IDataProtector _protector;
        private readonly ILogger<BakersController> _logger;

        // cookie name you will use
        private const string BakerCookieName = "baker_auth";
        // cookie lifetime in minutes
        private const int BakerCookieMinutes = 60;

        public BakersController(AppDbContext db,
                                 IPasswordHasher<Baker> hasher,
                                 IDataProtectionProvider dpProvider,
                                 ILogger<BakersController> logger)
        {
            _db = db;
            _hasher = hasher;
            _protector = dpProvider.CreateProtector("BakerAuthV1");
            _logger = logger;
        }

        // GET /api/bakers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bakers = await _db.Bakers
                .OrderBy(b => b.BakerName)
                .Select(b => new { id = b.Id, bakerName = b.BakerName, hasPin = b.PinHash != null })
                .ToListAsync();

            return Ok(new { success = true, data = bakers });
        }

        // POST /api/bakers/login
        // body: { bakerName: "...", pin: "1234" }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.BakerName) || string.IsNullOrEmpty(req.Pin))
                return BadRequest(new { success = false, message = "bakerName and pin required" });

            var baker = await _db.Bakers.FirstOrDefaultAsync(b => b.BakerName == req.BakerName);
            if (baker == null)
                return NotFound(new { success = false, message = "Unknown baker" });

            // If no pin registered yet -> register (hash and store)
            if (string.IsNullOrEmpty(baker.PinHash))
            {
                baker.PinHash = _hasher.HashPassword(baker, req.Pin);
                await _db.SaveChangesAsync();
                // issue cookie and return created
                IssueBakerCookie(baker.Id);
                return Ok(new { success = true, created = true, message = "PIN registered. You're now logged in." });
            }

            // verify
            var result = _hasher.VerifyHashedPassword(baker, baker.PinHash, req.Pin);
            if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                // optionally rehash if needed
                if (result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    baker.PinHash = _hasher.HashPassword(baker, req.Pin);
                    await _db.SaveChangesAsync();
                }

                IssueBakerCookie(baker.Id);
                return Ok(new { success = true, created = false, message = "Login successful." });
            }

            return Unauthorized(new { success = false, message = "Invalid PIN." });
        }

        // GET /api/bakers/current
        [HttpGet("current")]
        public async Task<IActionResult> Current()
        {
            var bakerId = ValidateAndGetBakerIdFromCookie();
            if (bakerId == null) return Ok(new { success = true, data = (object?)null });

            var baker = await _db.Bakers.FindAsync(bakerId.Value);
            if (baker == null) return Ok(new { success = true, data = (object?)null });

            return Ok(new { success = true, data = new { id = baker.Id, bakerName = baker.BakerName, cookieId = baker.CookieId } });
        }

        private int? ValidateAndGetBakerIdFromCookie()
        {
            if (!Request.Cookies.TryGetValue(BakerCookieName, out var protectedValue)) return null;
            try
            {
                var unprotected = _protector.Unprotect(protectedValue);
                // format: bakerId|ticks
                var parts = unprotected.Split('|');
                if (parts.Length != 2) return null;
                if (!int.TryParse(parts[0], out var bakerId)) return null;
                if (!long.TryParse(parts[1], out var ticks)) return null;
                var expiry = DateTimeOffset.FromUnixTimeSeconds(ticks);
                if (DateTimeOffset.UtcNow > expiry) return null;
                return bakerId;
            }
            catch
            {
                return null;
            }
        }

        private void IssueBakerCookie(int bakerId)
        {
            var expiry = DateTimeOffset.UtcNow.AddMinutes(BakerCookieMinutes).ToUnixTimeSeconds();
            var payload = $"{bakerId}|{expiry}";
            var protectedValue = _protector.Protect(payload);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(BakerCookieMinutes).UtcDateTime
            };
            Response.Cookies.Append(BakerCookieName, protectedValue, cookieOptions);
        }

    }
}
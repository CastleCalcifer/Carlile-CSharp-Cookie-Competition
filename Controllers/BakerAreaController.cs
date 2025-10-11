// Controllers/BakerAreaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Carlile_Cookie_Competition.Controllers
{
    [ApiController]
    [Route("baker")]
    public class BakerAreaController : ControllerBase
    {
        private readonly IDataProtector _protector;
        private readonly IWebHostEnvironment _env;
        private const string BakerCookieName = "baker_auth";
        private const string ProtectorPurpose = "BakerAuthV1";

        public BakerAreaController(IDataProtectionProvider dpProvider, IWebHostEnvironment env)
        {
            _protector = dpProvider.CreateProtector(ProtectorPurpose);
            _env = env;
        }

        // GET /baker/area
        [HttpGet("area")]
        public IActionResult Area()
        {
            var bakerId = ValidateAndGetBakerIdFromCookie();
            if (bakerId == null)
                return Redirect("/baker-select.html"); // or return Unauthorized()

            // Serve file from a protected folder (NOT under wwwroot)
            var protectedPath = Path.Combine(_env.ContentRootPath, "Protected", "baker-area.html");
            if (!System.IO.File.Exists(protectedPath))
                return NotFound();

            // return the file with content-type text/html
            var bytes = System.IO.File.ReadAllBytes(protectedPath);
            return File(bytes, "text/html");
        }

        private int? ValidateAndGetBakerIdFromCookie()
        {
            if (!Request.Cookies.TryGetValue(BakerCookieName, out var protectedValue)) return null;
            try
            {
                var unprotected = _protector.Unprotect(protectedValue);
                var parts = unprotected.Split('|');
                if (parts.Length != 2) return null;
                if (!int.TryParse(parts[0], out var bakerId)) return null;
                if (!long.TryParse(parts[1], out var expiry)) return null;
                if (DateTimeOffset.UtcNow > DateTimeOffset.FromUnixTimeSeconds(expiry)) return null;
                return bakerId;
            }
            catch { return null; }
        }
    }
}

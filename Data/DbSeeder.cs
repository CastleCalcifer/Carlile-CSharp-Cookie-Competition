using System.Linq;
using Carlile_Cookie_Competition.Models;

namespace Carlile_Cookie_Competition.Data
{
    public static class DbSeeder
    {
        public static void EnsureSeed(AppDbContext db)
        {
            // If there are already cookies for 2024, skip
            if (db.Cookies.Any(c => c.Year == 2024))
                return;

            db.Cookies.AddRange(
                new Cookie { Year = 2024, CookieName = "Chocolate Chip", ImageUrl = "/images/chocchip.jpg", Score = 0 },
                new Cookie { Year = 2024, CookieName = "Oatmeal Raisin", ImageUrl = "/images/oatmeal.jpg", Score = 0 },
                new Cookie { Year = 2024, CookieName = "Peanut Butter", ImageUrl = "/images/peanut.jpg", Score = 0 },
                new Cookie { Year = 2024, CookieName = "Snickerdoodle", ImageUrl = "/images/snickerdoodle.jpg", Score = 0 }
            );

            db.SaveChanges();
        }
    }
}

using System.Linq;
using Carlile_Cookie_Competition.Models;

namespace Carlile_Cookie_Competition.Data
{
    public static class DbSeeder
    {
        public static void EnsureSeed(AppDbContext db)
        {
            if (db.Cookies.Any(c => c.Year == 2024)) return;

            db.Cookies.AddRange(
                new Cookie("Gingerbread", 2024, "/images/gingerbread-2024.jpg", "Michael"),
                new Cookie("Tiramisu", 2024, "/images/tiramisu-2024.jpg", "Maria"),
                new Cookie("Grocery Store Chocolate", 2024, "/images/grocerystorechocolate-2024.jpg", "Christopher"),
                new Cookie("Snickerdoodle", 2024, "/images/snickerdoodle.jpg", "Dave")
            );

            db.SaveChanges();
        }
    }
}

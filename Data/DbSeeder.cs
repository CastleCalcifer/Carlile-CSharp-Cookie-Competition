using Carlile_Cookie_Competition.Models;

namespace Carlile_Cookie_Competition.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Cookies.Any())
            {
                context.Cookies.AddRange(
                    new Cookie { Id = 1, CookieName = "Chocolate Chip", BakerId = "baker1" },
                    new Cookie { Id = 2, CookieName = "Gingerbread", BakerId = "baker2" }
                );
                context.SaveChanges();
            }
        }
    }
}
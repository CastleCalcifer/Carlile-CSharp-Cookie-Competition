using System;
using System.Linq;
using Carlile_Cookie_Competition.Models;
using Microsoft.EntityFrameworkCore;

namespace Carlile_Cookie_Competition.Data
{
    public static class DbSeeder
    {
        public static void EnsureSeed(AppDbContext db)
        {
            // Wrap in a transaction when possible
            using var tx = db.Database.BeginTransaction();

            try
            {
                // Seed cookies for 2024 if any missing
                if (!db.Cookies.Any(c => c.Year == 2024 && c.CookieName == "Gingerbread"))
                {
                    db.Cookies.AddRange(
                        new Cookie("Gingerbread", 2024, "images/gingerbread-2024.jpg", "Michael"),
                        new Cookie("Tiramisu", 2024, "images/tiramisu-2024.jpg", "Maria"),
                        new Cookie("Grocery Store Chocolate", 2024, "images/grocerystorechocolate-2024.jpg", "Christopher"),
                        new Cookie("Italian Ricotta", 2024, "images/italianricotta-2024.jpg", "Bridget")
                    );
                    db.SaveChanges();
                }

                // Baker names are hard coded since this is only for my family's use
                var bakerNames = new[] { "Christopher", "Bridget", "Michael", "Maria" };

                foreach (var name in bakerNames)
                {
                    var existingBaker = db.Bakers.FirstOrDefault(b => b.BakerName == name);
                    if (existingBaker == null)
                    {
                        // try to find cookie for baker (same year)
                        var cookie = db.Cookies.FirstOrDefault(c => c.Year == 2024 && c.BakerName == name);
                        var cookieId = cookie?.Id;
                        var baker = new Baker(name, cookieId);
                        db.Bakers.Add(baker);
                        db.SaveChanges();
                    }
                    else
                    {
                        // ensure CookieId is set if cookie exists and Baker.CookieId is null
                        if (existingBaker.CookieId == null)
                        {
                            var cookie = db.Cookies.FirstOrDefault(c => c.Year == 2024 && c.BakerName == name);
                            if (cookie != null)
                            {
                                existingBaker.CookieId = cookie.Id;
                                db.Bakers.Update(existingBaker);
                                db.SaveChanges();
                            }
                        }
                    }
                }

                tx.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DbSeeder] Error during seed: {ex}");
                try { tx.Rollback(); } catch { }
                throw;
            }
        }
    }
}

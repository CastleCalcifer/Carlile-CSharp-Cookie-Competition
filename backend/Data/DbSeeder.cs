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
                // Seed cookies for 2025 if any missing
                if (!db.Cookies.Any(c => c.Year == 2025 && c.CookieName == "Oreo"))
                {
                    db.Cookies.AddRange(
                        new Cookie("Corn Earl Gray Icecream", 2025, "images/cornearlgray-2025.jpg", "Michael"),
                        new Cookie("Oatmeal Creampie", 2025, "images/oatmealcreampie-2025.jpg", "Maria"),
                        new Cookie("Chocolate Coffee Icecream", 2025, "images/coffeeicecream-2025.jpg", "Christopher"),
                        new Cookie("Oreo", 2025, "images/Oreo-2025.png", "Bridget")
                    );
                    db.SaveChanges();
                }

                // Baker names are hard coded since this is only for my family's use
                var bakerNames = new[] { "Christopher", "Bridget", "Michael", "Maria", "Carleen" };

                foreach (var name in bakerNames)
                {
                    var existingBaker = db.Bakers.FirstOrDefault(b => b.BakerName == name);
                    if (existingBaker == null)
                    {
                        // try to find cookie for baker (same year)
                        var cookie = db.Cookies.FirstOrDefault(c => c.Year == 2025 && c.BakerName == name);
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
                            var cookie = db.Cookies.FirstOrDefault(c => c.Year == 2025 && c.BakerName == name);
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

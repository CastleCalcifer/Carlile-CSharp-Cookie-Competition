//using System.Collections.Generic;
//using Carlile_Cookie_Competition.Models;
//using Carlile_Cookie_Competition.Services;
//using Microsoft.EntityFrameworkCore;

//public static class TestHelper
//{
//    public static CookieService CreateCookieServiceWithSampleData()
//    {
//        // Set up in-memory EF Core context
//        var options = new DbContextOptionsBuilder<CookieDbContext>()
//            .UseInMemoryDatabase(databaseName: "TestCookieDb")
//            .Options;

//        var context = new CookieDbContext(options);

//        // Seed sample bakers
//        var bakers = new List<Baker>
//        {
//            new Baker { Id = "baker1", Name = "Alice" },
//            new Baker { Id = "baker2", Name = "Bob" }
//        };
//        context.Bakers.AddRange(bakers);

//        // Seed sample cookies
//        var cookies = new List<Cookie>
//        {
//            new Cookie { Id = 1, Name = "Chocolate Chip", BakerId = "baker1" },
//            new Cookie { Id = 2, Name = "Gingerbread", BakerId = "baker2" }
//        };
//        context.Cookies.AddRange(cookies);

//        // Save changes
//        context.SaveChanges();

//        // Return service instance
//        return new CookieService(context);
//    }
//}
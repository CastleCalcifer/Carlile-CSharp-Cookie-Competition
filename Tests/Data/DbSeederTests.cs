using Microsoft.EntityFrameworkCore;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Models;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Data
{
    public class DbSeederTests : IDisposable
    {
        private readonly AppDbContext _context;

        public DbSeederTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
        }

        [Fact]
        public void EnsureSeed_WithEmptyDatabase_ShouldCreateCookiesAndBakers()
        {
            // Act
            DbSeeder.EnsureSeed(_context);

            // Assert
            var cookies = _context.Cookies.ToList();
            var bakers = _context.Bakers.ToList();

            cookies.Should().HaveCount(4);
            bakers.Should().HaveCount(4);

            // Verify cookies
            cookies.Should().Contain(c => c.CookieName == "Gingerbread" && c.Year == 2024);
            cookies.Should().Contain(c => c.CookieName == "Tiramisu" && c.Year == 2024);
            cookies.Should().Contain(c => c.CookieName == "Grocery Store Chocolate" && c.Year == 2024);
            cookies.Should().Contain(c => c.CookieName == "Italian Ricotta" && c.Year == 2024);

            // Verify bakers
            bakers.Should().Contain(b => b.BakerName == "Christopher");
            bakers.Should().Contain(b => b.BakerName == "Bridget");
            bakers.Should().Contain(b => b.BakerName == "Michael");
            bakers.Should().Contain(b => b.BakerName == "Maria");
        }

        [Fact]
        public void EnsureSeed_WithExistingData_ShouldNotDuplicate()
        {
            // Arrange
            DbSeeder.EnsureSeed(_context);
            var initialCookieCount = _context.Cookies.Count();
            var initialBakerCount = _context.Bakers.Count();

            // Act
            DbSeeder.EnsureSeed(_context);

            // Assert
            _context.Cookies.Count().Should().Be(initialCookieCount);
            _context.Bakers.Count().Should().Be(initialBakerCount);
        }

        [Fact]
        public void EnsureSeed_ShouldLinkBakersToCookies()
        {
            // Act
            DbSeeder.EnsureSeed(_context);

            // Assert
            var bakers = _context.Bakers.Include(b => b.Cookie).ToList();

            var michaelBaker = bakers.First(b => b.BakerName == "Michael");
            var michaelCookie = _context.Cookies.First(c => c.CookieName == "Gingerbread");
            michaelBaker.CookieId.Should().Be(michaelCookie.Id);

            var mariaBaker = bakers.First(b => b.BakerName == "Maria");
            var mariaCookie = _context.Cookies.First(c => c.CookieName == "Tiramisu");
            mariaBaker.CookieId.Should().Be(mariaCookie.Id);

            var christopherBaker = bakers.First(b => b.BakerName == "Christopher");
            var christopherCookie = _context.Cookies.First(c => c.CookieName == "Grocery Store Chocolate");
            christopherBaker.CookieId.Should().Be(christopherCookie.Id);

            var bridgetBaker = bakers.First(b => b.BakerName == "Bridget");
            var bridgetCookie = _context.Cookies.First(c => c.CookieName == "Italian Ricotta");
            bridgetBaker.CookieId.Should().Be(bridgetCookie.Id);
        }

        [Fact]
        public void EnsureSeed_WithExistingBakerMissingCookieId_ShouldUpdateCookieId()
        {
            // Arrange
            var baker = new Baker("Christopher", null);
            _context.Bakers.Add(baker);
            _context.SaveChanges();

            // Act
            DbSeeder.EnsureSeed(_context);

            // Assert
            var updatedBaker = _context.Bakers.First(b => b.BakerName == "Christopher");
            updatedBaker.CookieId.Should().NotBeNull();
            
            var expectedCookie = _context.Cookies.First(c => c.CookieName == "Grocery Store Chocolate");
            updatedBaker.CookieId.Should().Be(expectedCookie.Id);
        }

        [Fact]
        public void EnsureSeed_ShouldSetCorrectCookieImages()
        {
            // Act
            DbSeeder.EnsureSeed(_context);

            // Assert
            var cookies = _context.Cookies.ToList();

            var gingerbread = cookies.First(c => c.CookieName == "Gingerbread");
            gingerbread.Image.Should().Be("images/gingerbread-2024.jpg");

            var tiramisu = cookies.First(c => c.CookieName == "Tiramisu");
            tiramisu.Image.Should().Be("images/tiramisu-2024.jpg");

            var groceryStore = cookies.First(c => c.CookieName == "Grocery Store Chocolate");
            groceryStore.Image.Should().Be("images/grocerystorechocolate-2024.jpg");

            var italianRicotta = cookies.First(c => c.CookieName == "Italian Ricotta");
            italianRicotta.Image.Should().Be("images/italianricotta-2024.jpg");
        }

        [Fact]
        public void EnsureSeed_ShouldSetCorrectBakerNames()
        {
            // Act
            DbSeeder.EnsureSeed(_context);

            // Assert
            var cookies = _context.Cookies.ToList();

            var gingerbread = cookies.First(c => c.CookieName == "Gingerbread");
            gingerbread.BakerName.Should().Be("Michael");

            var tiramisu = cookies.First(c => c.CookieName == "Tiramisu");
            tiramisu.BakerName.Should().Be("Maria");

            var groceryStore = cookies.First(c => c.CookieName == "Grocery Store Chocolate");
            groceryStore.BakerName.Should().Be("Christopher");

            var italianRicotta = cookies.First(c => c.CookieName == "Italian Ricotta");
            italianRicotta.BakerName.Should().Be("Bridget");
        }

        [Fact]
        public void EnsureSeed_ShouldSetDefaultValues()
        {
            // Act
            DbSeeder.EnsureSeed(_context);

            // Assert
            var cookies = _context.Cookies.ToList();
            var bakers = _context.Bakers.ToList();

            // All cookies should have default values
            foreach (var cookie in cookies)
            {
                cookie.Score.Should().Be(0);
                cookie.CreativePoints.Should().Be(0);
                cookie.PresentationPoints.Should().Be(0);
                cookie.Year.Should().Be(2024);
            }

            // All bakers should have default values
            foreach (var baker in bakers)
            {
                baker.HasVoted.Should().BeFalse();
                baker.PinHash.Should().BeNull();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

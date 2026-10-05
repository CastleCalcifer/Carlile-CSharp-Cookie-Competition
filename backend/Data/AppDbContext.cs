using Microsoft.EntityFrameworkCore;
using Carlile_Cookie_Competition.Models;

namespace Carlile_Cookie_Competition.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<Cookie> Cookies { get; set; } = null!;
        public DbSet<Baker> Bakers { get; set; } = null!;
        public DbSet<YearRecord> Years { get; set; } = null!;

        public DbSet<Vote> Votes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Baker.CookieId is a nullable FK to Cookie.Id. Enforce one-to-one relationship:
            modelBuilder.Entity<Baker>(b =>
            {
                // ensure CookieId column maps as optional FK
                b.HasOne(x => x.Cookie)
                 .WithOne(x => x.Baker)                  // Cookie.Baker navigation
                 .HasForeignKey<Baker>(x => x.CookieId) // Baker.CookieId is the FK
                 .OnDelete(DeleteBehavior.SetNull);  

                // Make cookie_id unique so only one baker can reference a cookie
                b.HasIndex(x => x.CookieId)
                 .IsUnique();

                b.HasIndex(x => x.BakerName)
                 .IsUnique();
            });

            // Additional configuration for Cookie if desired
            modelBuilder.Entity<Cookie>(c =>
            {
                c.Property(x => x.Image).HasDefaultValue("/images/placeholder.jpg");
            });
        }
    }
}

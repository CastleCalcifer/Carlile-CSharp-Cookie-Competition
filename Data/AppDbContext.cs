using Microsoft.EntityFrameworkCore;
using CookieVotingApi.Models;
using Carlile_Cookie_Competition.Models;

namespace CookieVotingApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<Cookie> Cookies { get; set; } = null!;
        public DbSet<Baker> Bakers { get; set; } = null!;
        public DbSet<YearRecord> Years { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Baker>()
                .HasOne(b => b.Cookie)
                .WithMany(c => c.Bakers)
                .HasForeignKey(b => b.CookieId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}

using Carlile_Cookie_Competition.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
    public DbSet<Cookie> Cookies => Set<Cookie>();
    public DbSet<Vote> Votes => Set<Vote>();
}

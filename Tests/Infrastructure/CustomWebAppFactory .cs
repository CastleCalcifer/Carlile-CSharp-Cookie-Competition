using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Carlile_Cookie_Competition.Data; // AppDbContext namespace
using Carlile_Cookie_Competition.Models;

namespace Carlile_Cookie_Competition.Tests.Infrastructure
{
    // TStartup should be the type of your Startup class. Change namespace if needed.
    public class BakerLoginTests<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        // Create a unique DB name per factory instance to avoid cross-test collisions.
        public string InMemoryDbName { get; } = $"TestDb_{Guid.NewGuid():N}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development"); // use Development settings if needed

            builder.ConfigureServices(services =>
            {
                // Remove the existing AppDbContext registration (if any)
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // Add EF InMemory DB for tests
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(InMemoryDbName);
                });

                // Build the service provider and seed the DB
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var sc = scope.ServiceProvider;
                var db = sc.GetRequiredService<AppDbContext>();
                // Ensure created
                db.Database.EnsureCreated();
                // Optionally seed common test data (we do minimal here)
            });
        }
    }
}

using Carlile_Cookie_Competition.Data;   // where AppDbContext & DbSeeder live
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CookieVotingApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // -------------------- ConfigureServices --------------------
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Web API controllers
            services.AddControllers();

            // Swagger/OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // EF Core (SQLite)
            var conn = Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=votes.db";
            services.AddDbContext<AppDbContext>(opts => opts.UseSqlite(conn));

            // CORS for local front-end
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalDevFront", policy =>
                    policy.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });
        }

        // -------------------- Configure middleware pipeline --------------------
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();   // serves files in wwwroot/
            app.UseRouting();
            app.UseCors("AllowLocalDevFront");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            // Run migrations + seed data on startup
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // in Startup.Configure(...) after creating scope or in Program after Build() but before Run()
            // Use only in Development
            if (env.IsDevelopment())
            {
                try
                {
                    // Option A: delete the database file (equivalent to EnsureDeleted)
                    // This will physically remove the file used by SQLite, then Migrate will recreate it.
                    var connString = db.Database.GetDbConnection().ConnectionString;
                    Console.WriteLine($"[DevReset] DB connection: {connString}");

                    // If you're using a file-based SQLite connection like "Data Source=votes.db"
                    // we can call EnsureDeleted (EF) which works cross-provider.
                    db.Database.EnsureDeleted();   // destroy any existing DB (dev only)

                    // Recreate schema from migrations (preferred if you use migrations)
                    db.Database.Migrate();

                    // Or, if you are not using migrations and prefer a quick create:
                    // db.Database.EnsureCreated();

                    // Seed demo data
                    DbSeeder.EnsureSeed(db);

                    Console.WriteLine("[DevReset] DB deleted, migrated, and seeded.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DevReset] Error resetting DB: {ex}");
                    // Consider rethrowing in dev so you notice the failure:
                    throw;
                }
            }
            else
            {
                db.Database.Migrate();
                DbSeeder.EnsureSeed(db);
            }

            }
    }
}

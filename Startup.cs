using Carlile_Cookie_Competition.Data;   // where AppDbContext & DbSeeder live
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Carlile_Cookie_Competition.Models; // Baker model

using System;

namespace Carlile_Cookie_Competition
{
    /// <summary>
    /// Configures services and middleware for the Cookie Voting API.
    /// </summary>
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Registers services for dependency injection.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            var conn = Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=votes.db";
            services.AddDbContext<AppDbContext>(opts => opts.UseSqlite(conn));

            services.AddDataProtection();
            services.AddScoped<IPasswordHasher<Baker>, PasswordHasher<Baker>>();

            // Allow CORS for local development front-end
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalDevFront", policy =>
                    policy.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });
        }

        /// <summary>
        /// Configures the HTTP request pipeline and runs DB migrations/seeding.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowLocalDevFront");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Ensure database is migrated and seeded on startup
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // IF YOU NEED TO TEST THE DATABASE UNCOMMENT THIS
            //if (env.IsDevelopment())
            //{
            //    try
            //    {
            //        var connString = db.Database.GetDbConnection().ConnectionString;
            //        Console.WriteLine($"[DevReset] DB connection: {connString}");

            //        db.Database.EnsureDeleted(); // Reset DB for development
            //        db.Database.Migrate();
            //        DbSeeder.EnsureSeed(db);

            //        Console.WriteLine("[DevReset] DB deleted, migrated, and seeded.");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"[DevReset] Error resetting DB: {ex}");
            //        throw;
            //    }
            //}
            //else
            //{
            //    db.Database.Migrate();
            //    DbSeeder.EnsureSeed(db);
            //}
            // for production keep these uncommented.
            db.Database.Migrate();
            DbSeeder.EnsureSeed(db);
        }
    }
}

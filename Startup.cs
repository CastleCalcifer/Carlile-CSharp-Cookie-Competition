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
            db.Database.Migrate();
            DbSeeder.EnsureSeed(db);
        }
    }
}

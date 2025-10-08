using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CookieVotingApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Builds and configures the host (Kestrel web server, config, DI, etc.)
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Startup contains all service registrations & middleware pipeline
                    webBuilder.UseStartup<Startup>();
                });
    }
}

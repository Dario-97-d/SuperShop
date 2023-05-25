using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperShop.Web.Data;

namespace SuperShop.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuild = CreateHostBuilder(args).Build();
            RunDbSeeding(hostBuild);
            hostBuild.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static void RunDbSeeding(IHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            
            using var scope = scopeFactory.CreateScope();

            var seeder = scope.ServiceProvider.GetService<SeedDb>();
            seeder.SeedAsync().Wait();
        }
    }
}

using Avalonia;
using Avalonia.ReactiveUI;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace App
{
    class Program
    {
        // This is your existing Avalonia setup
        [STAThread]
        public static void Main(string[] args)
        {
            // 1. Configure the host builder
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Add DbContext with configuration from appsettings.json
                    services.AddDbContext<AppDbContext>(options => 
                        options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
                })
                .Build();

            // 2. Run the seeder
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                Infrastructure.Seeders.DatabaseSeeder.Seed(dbContext);
            }

            // 3. Start Avalonia as before
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
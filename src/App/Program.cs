using App.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace App;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            Debug.WriteLine("Program.Main started");
            var services = new ServiceCollection();
            services.AddHttpClient("CarInsuranceApi", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5000");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddSingleton<AppState>();
            services.AddSingleton<HomePageViewModel>();
            services.AddSingleton<CarPageViewModel>();
            services.AddSingleton<AuthPageViewModel>();
            services.AddSingleton<AboutPageViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            Debug.WriteLine("DI services registered");

            var serviceProvider = services.BuildServiceProvider();
            AppState.ServiceProvider = serviceProvider;
            Debug.WriteLine("AppState.ServiceProvider set");

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
            Debug.WriteLine("Avalonia app started");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Program.Main error: {ex}");
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
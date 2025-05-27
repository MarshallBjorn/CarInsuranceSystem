using App.Factories;
using App.Support;
using App.ViewModels;
using App.ViewModels.AuthPageViewModels;
using App.ViewModels.FirmPageViewModels;
using Avalonia;
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
                client.BaseAddress = new Uri("http://localhost:6000");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .ConfigureHttpClient(client =>
            {
                var token = TokenStorage.Token;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                } 
            });

            services.AddSingleton<IAuthViewModelFactory, AuthModelFactory>();
            services.AddSingleton<IFirmViewModelFactory, FirmViewModelFactory>();

            services.AddSingleton<AppState>();
            services.AddSingleton<HomePageViewModel>();
            services.AddSingleton<CarPageViewModel>();
            services.AddSingleton<AuthPageViewModel>();
            services.AddSingleton<FirmPageViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<UserPageViewModel>();
            services.AddTransient<FirmViewModel>();
            services.AddTransient<NewFirmViewModel>();
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
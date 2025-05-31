using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace App.ViewModels;

public partial class HomePageViewModel : ViewModelBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    [ObservableProperty] private int _firms;
    [ObservableProperty] private int _insurances;
    [ObservableProperty] private int _users;

    public HomePageViewModel()
    {
        _httpClientFactory = AppState.ServiceProvider?.GetRequiredService<IHttpClientFactory>()
            ?? throw new ArgumentNullException(nameof(HttpClientFactory));
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadInfoAsync();
        AppState.OnInsuranceChange += async () => await LoadInfoAsync();
        AppState.OnFirmChange += async () => await LoadInfoAsync();
        AppState.OnCarChange += async () => await LoadInfoAsync();
    }

    private async Task LoadInfoAsync()
    {
        var client = _httpClientFactory.CreateClient("CarInsuranceApi");

        var response = await client.GetAsync($"api/Firm/count");
        int firms = await response.Content.ReadFromJsonAsync<int>();
        Firms = firms;

        response = await client.GetAsync($"api/InsuranceTypes/count");
        int insurances = await response.Content.ReadFromJsonAsync<int>();
        Insurances = insurances;

        response = await client.GetAsync($"api/User/count");
        int users = await response.Content.ReadFromJsonAsync<int>();
        Users = users;
    }
}
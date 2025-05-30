using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.Factories;
using App.Support;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace App.ViewModels.FirmPageViewModels;

public partial class NewInsuranceTypeViewModel : ViewModelBase
{
    private readonly IHttpClientFactory _httpCLientFactory;
    private readonly HttpClient _client;
    private readonly IFirmViewModelFactory _factory;
    public Action? OnNewInsuranceAdd;

    [ObservableProperty] private ObservableCollection<FirmViewModel> _firms = new();
    [ObservableProperty] private FirmViewModel? _selectedFirm;

    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _policyNumber = string.Empty;
    [ObservableProperty] private string _policyDescription = string.Empty;

    [ObservableProperty] private string _messageText = string.Empty;

    public NewInsuranceTypeViewModel(IHttpClientFactory httpClientFactory, IFirmViewModelFactory factory)
    {
        _factory = factory;
        _httpCLientFactory = httpClientFactory;
        _client = _httpCLientFactory.CreateClient("CarInsuranceApi");

        _ = LoadFirmsAsync();
    }

    private async Task LoadFirmsAsync()
    {
        var user = AppState.LoggedInUser;
        var token = TokenStorage.Token;

        if (user is null || string.IsNullOrWhiteSpace(token))
        {
            MessageText = "You have to loggin first";
            return;
        }

        try
        {
            var response = await _client.GetAsync($"api/Firm/user/{user.Id}");

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageText = $"{response.StatusCode}: {response.Content}";
                return;
            }

            var firms = await response.Content.ReadFromJsonAsync<Firm[]>();

            if (firms == null)
            {
                MessageText = "Failed to load firms";
                return;
            }

            Firms = new ObservableCollection<FirmViewModel>(
                firms.Select(firm => _factory.CreateEdit(firm))
            );
        }
        catch (Exception ex)
        {
            MessageText = $"Failed to load cars. {ex.Message}";
        }
    }
    
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedFirm == null)
        {
            MessageText = "Please select a firm.";
            return;
        }

        var newInsurance = new InsuranceType
        {
            Name = Name,
            PolicyNumber = PolicyNumber,
            PolicyDescription = PolicyDescription,
            FirmId = SelectedFirm.Firm.Id
        };

        try
        {
            var response = await _client.PostAsJsonAsync("api/InsuranceTypes", newInsurance);
            if (!response.IsSuccessStatusCode)
            {
                MessageText = $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                return;
            }

            MessageText = "Insurance type added successfully.";
            OnNewInsuranceAdd?.Invoke();
            AppState.RaiseInsurance();
        }
        catch (Exception ex)
        {
            MessageText = $"Unexpected error: {ex.Message}";
        }
    }
}
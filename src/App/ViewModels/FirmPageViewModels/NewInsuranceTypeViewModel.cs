using System;
using System.Collections.Generic;
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
using Core.Validators;
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
    [ObservableProperty] private string _price = string.Empty;

    [ObservableProperty] private string _messageText = string.Empty;

    private readonly List<string> _nameErrors = new();
    private readonly List<string> _policyNumberErrors = new();
    private readonly List<string> _policyDescriptionErrors = new();
    private readonly List<string> _priceErrors = new();

    public string NameErrors => _nameErrors.Count > 0 ? string.Join("\n", _nameErrors) : string.Empty;
    public string PolicyNumberErrors => _policyNumberErrors.Count > 0 ? string.Join("\n", _policyNumberErrors) : string.Empty;
    public string PolicyDescriptionErrors => _policyDescriptionErrors.Count > 0 ? string.Join("\n", _policyDescriptionErrors) : string.Empty;
    public string PriceErrors => _priceErrors.Count > 0 ? string.Join("\n", _priceErrors) : string.Empty;

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
        _nameErrors.Clear();
        _policyNumberErrors.Clear();
        _policyDescriptionErrors.Clear();
        _priceErrors.Clear();
        OnPropertyChanged(nameof(NameErrors));
        OnPropertyChanged(nameof(PolicyNumberErrors));
        OnPropertyChanged(nameof(PolicyDescriptionErrors));
        OnPropertyChanged(nameof(PriceErrors));


        if (SelectedFirm == null)
        {
            MessageText = "Please select a firm.";
            return;
        }

        if (!decimal.TryParse(Price, out var parsedPrice))
        {
            _priceErrors.Add("Price must be a valid number.");
            OnPropertyChanged(nameof(PriceErrors));
            return;
        }

        var validator = new InsuranceTypeValidator();
        var newInsurance = new InsuranceType
        {
            Name = Name,
            PolicyNumber = PolicyNumber,
            PolicyDescription = PolicyDescription,
            FirmId = SelectedFirm.Firm.Id,
            Price = decimal.Parse(Price)
        };

        var result = validator.Validate(newInsurance);

        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                switch (error.PropertyName)
                {
                    case "Name":
                        _nameErrors.Add(error.ErrorMessage);
                        break;
                    case "PolicyNumber":
                        _policyNumberErrors.Add(error.ErrorMessage);
                        break;
                    case "PolicyDescription":
                        _policyDescriptionErrors.Add(error.ErrorMessage);
                        break;
                    case "Price":
                        _priceErrors.Add(error.ErrorMessage);
                        return;
                }
            }

            OnPropertyChanged(nameof(NameErrors));
            OnPropertyChanged(nameof(PolicyNumberErrors));
            OnPropertyChanged(nameof(PolicyDescriptionErrors));
            OnPropertyChanged(nameof(PriceErrors));
            return;
        }

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
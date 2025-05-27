using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.Validators;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;

namespace App.ViewModels.FirmPageViewModels;

public partial class FirmViewModel : ViewModelBase
{
    private readonly FirmPageViewModel _firmPageViewModel;
    private readonly IHttpClientFactory _httpClientFactory;
    public Action? OnFirmEdited { get; set; }

    [ObservableProperty] private string _name;
    [ObservableProperty] private string _countryCode;
    [ObservableProperty] private string _messageText = string.Empty;

    private readonly List<string> _nameErrors = new();
    private readonly List<string> _countryCodeErrors = new();

    public string NameErrors => _nameErrors.Count > 0 ? string.Join("\n", _nameErrors) : string.Empty;
    public string CountryCodeErrors => _countryCodeErrors.Count > 0 ? string.Join("\n", _countryCodeErrors) : string.Empty;

    public Firm Firm { get; }

    public FirmViewModel(Firm firm, FirmPageViewModel firmPageViewModel, IHttpClientFactory httpClientFactory)
    {
        Firm = firm;
        _firmPageViewModel = firmPageViewModel;
        _httpClientFactory = httpClientFactory;
        _name = Firm.Name;
        _countryCode = Firm.CountryCode;
    }

    [RelayCommand]
    private void ShowEdit()
    {
        _firmPageViewModel.FirmEditOpen(this);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        _nameErrors.Clear();
        _countryCodeErrors.Clear();
        OnPropertyChanged(nameof(NameErrors));
        OnPropertyChanged(nameof(CountryCodeErrors));

        var validator = new FirmValidator();
        var firmToValidate = new Firm { Name = Name, CountryCode = CountryCode };
        var result = validator.Validate(firmToValidate);

        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                if (error.PropertyName == "Name")
                    _nameErrors.Add(error.ErrorMessage);
                else if (error.PropertyName == "CountryCode")
                    _countryCodeErrors.Add(error.ErrorMessage);
            }

            OnPropertyChanged(nameof(NameErrors));
            OnPropertyChanged(nameof(CountryCodeErrors));
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("CarInsuranceApi");
            var updatedFirm = new Firm
            {
                Id = Firm.Id,
                Name = Name,
                CountryCode = CountryCode
            };

            var response = await client.PutAsJsonAsync($"api/Firm", updatedFirm);

            if (!response.IsSuccessStatusCode)
            {
                MessageText = $"Failed to update: {await response.Content.ReadAsStringAsync()}";
                return;
            }

            MessageText = "Firm updated successfully.";
            OnFirmEdited?.Invoke();
        }
        catch (Exception ex)
        {
            MessageText = $"Error: {ex.Message}";
        }
    }
}
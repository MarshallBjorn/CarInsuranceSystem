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

public partial class NewFirmViewModel : ViewModelBase
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _countryCode = string.Empty;
    [ObservableProperty] private string _messageText = string.Empty;

    private readonly List<string> _nameErrors = new();
    private readonly List<string> _countryCodeErrors = new();

    public string NameErrors => _nameErrors.Count > 0 ? string.Join("\n", _nameErrors) : string.Empty;
    public string CountryCodeErrors => _countryCodeErrors.Count > 0 ? string.Join("\n", _countryCodeErrors) : string.Empty;

    private readonly IHttpClientFactory _httpClientFactory;
    public Action? OnFirmAdded { get; set; }

    public NewFirmViewModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [RelayCommand]
    private async Task CreateFirm()
    {
        await AddFirmAsync();
    }

    private async Task AddFirmAsync()
    {
        var user = AppState.LoggedInUser;

        if (user is null)
        {
            MessageText = "You have to be logged in";
            return;
        }

        MessageText = string.Empty;
        _nameErrors.Clear();
        _countryCodeErrors.Clear();
        OnPropertyChanged(nameof(NameErrors));
        OnPropertyChanged(nameof(CountryCodeErrors));

        var firm = new Firm
        {
            Name = Name,
            UserId = user.Id,
            CountryCode = CountryCode
        };

        var validator = new FirmValidator();
        var validation = validator.Validate(firm);

        if (validation.IsValid)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CarInsuranceApi");
                var response = await client.PostAsJsonAsync("api/Firm", firm);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageText = result.Trim('"');
                    return;
                }

                MessageText = "Firm added successfully.";
                OnFirmAdded?.Invoke();
            }
            catch (Exception ex)
            {
                MessageText = ex.Message;
            }
        }
        else
        {
            foreach (var error in validation.Errors)
            {
                switch (error.PropertyName)
                {
                    case nameof(Firm.Name):
                        _nameErrors.Add(error.ErrorMessage);
                        break;
                    case nameof(Firm.CountryCode):
                        _countryCodeErrors.Add(error.ErrorMessage);
                        break;
                }
            }

            OnPropertyChanged(nameof(NameErrors));
            OnPropertyChanged(nameof(CountryCodeErrors));
        }
    }
}
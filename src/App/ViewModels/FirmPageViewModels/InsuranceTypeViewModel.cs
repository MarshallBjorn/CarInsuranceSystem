using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.Views.FirmPageViews;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;
using Core.Validators;

namespace App.ViewModels.FirmPageViewModels;

public partial class InsuranceTypeViewModel : ViewModelBase
{
    private readonly FirmPageViewModel _firmPageViewModel;
    private readonly FirmViewModel _firmViewModel;
    private readonly IHttpClientFactory _httpClientFactory;

    public Action? OnInsuranceEdited { get; set; }

    [ObservableProperty] private string _name;
    [ObservableProperty] private string _policyNumber;
    [ObservableProperty] private string _policyDescription;
    [ObservableProperty] private decimal _price;
    [ObservableProperty] private string _inputPrice;
    [ObservableProperty] private string _displayPrice;
    [ObservableProperty] private string _messageText = string.Empty;

    private readonly List<string> _nameErrors = new();
    private readonly List<string> _policyNumberErrors = new();
    private readonly List<string> _policyDescriptionErrors = new();
    private readonly List<string> _priceErrors = new();

    public string NameErrors => _nameErrors.Count > 0 ? string.Join("\n", _nameErrors) : string.Empty;
    public string PolicyNumberErrors => _policyNumberErrors.Count > 0 ? string.Join("\n", _policyNumberErrors) : string.Empty;
    public string PolicyDescriptionErrors => _policyDescriptionErrors.Count > 0 ? string.Join("\n", _policyDescriptionErrors) : string.Empty;
    public string PriceErrors => _priceErrors.Count > 0 ? string.Join("\n", _priceErrors) : string.Empty;

    public InsuranceType InsuranceType { get; }

    public InsuranceTypeViewModel(InsuranceType insuranceType, FirmViewModel firm, FirmPageViewModel firmPageViewModel, IHttpClientFactory httpClientFactory)
    {
        InsuranceType = insuranceType;
        _firmPageViewModel = firmPageViewModel;
        _firmViewModel = firm;
        _httpClientFactory = httpClientFactory;

        Name = InsuranceType.Name;
        PolicyNumber = InsuranceType.PolicyNumber;
        PolicyDescription = InsuranceType.PolicyDescription;
        Price = InsuranceType.Price;
        InputPrice = $"{Price}";
        DisplayPrice = $"{Price} PLN";
    }

    [RelayCommand]
    private void ShowEdit()
    {
        _firmPageViewModel.InsuranceEditOpen(this);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        _nameErrors.Clear();
        _policyNumberErrors.Clear();
        _policyDescriptionErrors.Clear();
        OnPropertyChanged(nameof(NameErrors));
        OnPropertyChanged(nameof(PolicyNumberErrors));
        OnPropertyChanged(nameof(PolicyDescriptionErrors));

        if (!decimal.TryParse(InputPrice, out var parsedPrice))
        {
            _priceErrors.Add("Price must be a valid number.");
            OnPropertyChanged(nameof(PriceErrors));
            return;
        }

        var validator = new InsuranceTypeValidator();
        var insuranceToValidate = new InsuranceType
        {
            Name = Name,
            PolicyNumber = PolicyNumber,
            PolicyDescription = PolicyDescription,
            Price = decimal.Parse(InputPrice)
        };
        var result = validator.Validate(insuranceToValidate);

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

            MessageText = "AAAAA";
            OnPropertyChanged(nameof(NameErrors));
            OnPropertyChanged(nameof(PolicyNumberErrors));
            OnPropertyChanged(nameof(PolicyDescriptionErrors));
            OnPropertyChanged(nameof(PriceErrors));
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("CarInsuranceApi");
            var updatedInsurance = new InsuranceType
            {
                Name = Name,
                PolicyNumber = PolicyNumber,
                PolicyDescription = PolicyDescription,
                FirmId = _firmViewModel.Firm.Id,
                Price = decimal.Parse(InputPrice)
            };

            var response = await client.PutAsJsonAsync($"api/InsuranceTypes/{InsuranceType.Id}", updatedInsurance);

            if (!response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                MessageText = $"Failed to update: {response.StatusCode}: {json}";
                Name = InsuranceType.FirmId.ToString();
                return;
            }

            MessageText = "Insurance updated successfully.";
            OnInsuranceEdited?.Invoke();
            AppState.RaiseInsurance();
        }
        catch (Exception ex)
        {
            MessageText = $"Error: {ex.Message}";
        }
    }
}
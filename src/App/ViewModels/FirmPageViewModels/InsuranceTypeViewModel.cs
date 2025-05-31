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
    [ObservableProperty] private string _messageText = string.Empty;

    private readonly List<string> _nameErrors = new();
    private readonly List<string> _policyNumberErrors = new();
    private readonly List<string> _policyDescriptionErrors = new();

    public string NameErrors => _nameErrors.Count > 0 ? string.Join("\n", _nameErrors) : string.Empty;
    public string PolicyNumberErrors => _policyNumberErrors.Count > 0 ? string.Join("\n", _policyNumberErrors) : string.Empty;
    public string PolicyDescriptionErrors => _policyDescriptionErrors.Count > 0 ? string.Join("\n", _policyDescriptionErrors) : string.Empty;

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

        var validator = new InsuranceTypeValidator();
        var insuranceToValidate = new InsuranceType
        {
            Name = Name,
            PolicyNumber = PolicyNumber,
            PolicyDescription = PolicyDescription,
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
                }
            }

            MessageText = "AAAAA";
            OnPropertyChanged(nameof(NameErrors));
            OnPropertyChanged(nameof(PolicyNumberErrors));
            OnPropertyChanged(nameof(PolicyDescriptionErrors));
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
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.Factories;
using App.Support;
using App.ViewModels.FirmPageViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;

namespace App.ViewModels;

public partial class FirmPageViewModel : ViewModelBase
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly HttpClient _client;
    private readonly IFirmViewModelFactory _factory;

    [ObservableProperty] private ObservableCollection<FirmViewModel> _firms = new();
    [ObservableProperty] private ObservableCollection<FirmViewModel> _filteredFirms = new();
    
    [ObservableProperty] private string _filterText = "";
    [ObservableProperty] private string _messageText = "";
    [ObservableProperty] private string _listText = "";

    [ObservableProperty] private bool _isList = false;
    [ObservableProperty] private bool _isEmpty = true;
    [ObservableProperty] private bool _buttonIsVisible = false;
    [ObservableProperty] public bool _isAnyPopupOpen = false;

    [ObservableProperty] private ViewModelBase _currentPopup = null!;

    public FirmPageViewModel(IFirmViewModelFactory factory, IHttpClientFactory httpClientFactory)
    {
        _clientFactory = httpClientFactory;
        _client = _clientFactory.CreateClient("CarInsuranceApi");
        _factory = factory;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadFirmsAsync();
        AppState.OnLogin += async () => await LoadFirmsAsync();

        AppState.OnLogOut += () =>
        {
            Firms.Clear();
            FilteredFirms.Clear();
            IsList = false;
            IsEmpty = true;
            ButtonIsVisible = false;
            ListText = "You have been logged out.";
        };
    }

    partial void OnFilterTextChanged(string value)
    {
        ApplyFirmFilter();
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

        ButtonIsVisible = true;

        try
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"api/Firm/user/{user.Id}");

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageText = $"{response.StatusCode}: {response.Content}";
                return;
            }

            var firms = await response.Content.ReadFromJsonAsync<Firm[]>();

            if (!IsList)
            {
                IsList ^= true;
                IsEmpty ^= true;
            }

            if (firms == null)
            {
                MessageText = "Failed to load firms";
                return;
            }

            Firms = new ObservableCollection<FirmViewModel>(
                firms.Select(firm => _factory.CreateEdit(firm))
            );

            ApplyFirmFilter();
        }
        catch (Exception ex)
        {
            MessageText = $"Failed to load cars. {ex.Message}";
        }
    }

    private void ApplyFirmFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            FilteredFirms = new ObservableCollection<FirmViewModel>(Firms);
            return;
        }

        var lower = FilterText.ToLowerInvariant();

        var filtered = Firms.Where(firmVm =>
            (firmVm.Firm.Name?.ToLower().Contains(lower) ?? false) ||
            (firmVm.Firm.CountryCode?.ToLower().Contains(lower) ?? false) ||
            (firmVm.Firm.CreatedAt.ToString("dd.MM.yyyy").ToLower().Contains(lower)) ||
            firmVm.Insurances.Any(ins =>
                (ins.InsuranceType.Name?.ToLower().Contains(lower) ?? false) ||
                (ins.InsuranceType.PolicyNumber?.ToLower().Contains(lower) ?? false) ||
                (ins.InsuranceType.PolicyDescription?.ToLower().Contains(lower) ?? false)
            )
        );

        FilteredFirms = new ObservableCollection<FirmViewModel>(filtered);
    }


    [RelayCommand]
    private void FirmAddOpen()
    {
        var firmAddVm = _factory.CreateAdd();
        firmAddVm.OnFirmAdded = ClosePopup;
        CurrentPopup = firmAddVm;
        IsAnyPopupOpen ^= true;
    }

    public void FirmEditOpen(FirmViewModel firmViewModel)
    {
        try
        {
            var firmEditVm = firmViewModel;
            firmEditVm.OnFirmEdited = ClosePopup;
            CurrentPopup = firmViewModel;
            IsAnyPopupOpen ^= true;
        }
        catch (Exception ex)
        {
            MessageText = $"Failed to open edit: {ex.Message}";
        }
    }

    [RelayCommand]
    public void InsuranceAddOpen()
    {
        var insuranceAddVm = _factory.CreateInsuranceAdd();
        insuranceAddVm.OnNewInsuranceAdd = ClosePopup;
        CurrentPopup = insuranceAddVm;
        IsAnyPopupOpen ^= true;
    }

    public void InsuranceEditOpen(InsuranceTypeViewModel insuranceTypeViewModel)
    {
        try
        {
            var insuranceEditVm = insuranceTypeViewModel;
            insuranceEditVm.OnInsuranceEdited = ClosePopup;
            CurrentPopup = insuranceEditVm;
            IsAnyPopupOpen ^= true;
        }
        catch (Exception ex)
        {
            MessageText = $"Failed to open edit: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ClosePopup()
    {
        _ = LoadFirmsAsync();
        MessageText = "Operation was success.";
        IsAnyPopupOpen ^= true;
    }
}
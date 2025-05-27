using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.Factories;
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
    [ObservableProperty] private ObservableCollection<FirmPageViewModel> _filteredFirms = new();
    [ObservableProperty] private string _filterText = "";
    [ObservableProperty] private string _messageText = "";
    [ObservableProperty] private string _listText = "";

    [ObservableProperty] private bool _isList = true;
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
    }

    private async Task LoadFirmsAsync()
    {
        try
        {
            var response = await _client.GetAsync("api/Firm");

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
                firms.Select(firm => new FirmViewModel(firm))
            );
        }
        catch (Exception ex)
        {
            MessageText = $"Failed to load cars. {ex.Message}";
        }
    }

    [RelayCommand]
    private void FirmAddOpen()
    {
        var firmAddVm = _factory.CreateAdd();
        firmAddVm.OnFirmAdded = ClosePopup;
        CurrentPopup = firmAddVm;
        IsAnyPopupOpen ^= true;
    }

    private void ClosePopup()
    {
        _ = LoadFirmsAsync();
        MessageText = "Operation was success.";
        IsAnyPopupOpen ^= true;
    }
}
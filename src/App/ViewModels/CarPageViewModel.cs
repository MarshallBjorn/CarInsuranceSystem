using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.Support;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;
using Core.Validators;
using FluentValidation;
using App.ViewModels.CarPageViewModels;
using System.Collections.Generic;

namespace App.ViewModels;

public partial class CarPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<CarViewModel> _cars = new();

    [ObservableProperty]
    private ObservableCollection<CarViewModel> _filteredCars = new();

    [ObservableProperty]
    private ObservableCollection<InsuranceViewModel>? _insurances;

    [ObservableProperty]
    private string _filterText = "";

    partial void OnFilterTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter(bool resetPage = true)
    {
        try
        {
            Debug.WriteLine("ApplyFilter called");

            if (resetPage)
                CurrentPage = 1;

            IEnumerable<CarViewModel> result;
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                result = Cars;
            }
            else
            {
                var lower = FilterText.ToLowerInvariant();
                result = Cars.Where(car =>
                    (car.Car.Model?.ToLower().Contains(lower) ?? false) ||
                    (car.Car.Mark?.ToLower().Contains(lower) ?? false) ||
                    (car.Car.VIN?.ToLower().Contains(lower) ?? false)
                );
            }

            FilteredCars = new ObservableCollection<CarViewModel>(result);

            UpdatePagedCars();
        }
        catch (Exception ex)
        {
            ErrorText = $"Failed to apply filter: {ex.Message}";
            Debug.WriteLine($"ApplyFilter error: {ex}");
        }
    }

    private void UpdatePagedCars()
    {
        try
        {
            if (FilteredCars == null || FilteredCars.Count == 0)
            {
                PagedCars = new ObservableCollection<CarViewModel>();
                TotalPages = 1;
                return;
            }

            TotalPages = (int)Math.Ceiling((double)FilteredCars.Count / ItemsPerPage);

            // Clamp current page within valid range
            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            var items = FilteredCars
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage);

            PagedCars = new ObservableCollection<CarViewModel>(items);
        }
        catch (Exception ex)
        {
            ErrorText = $"Pagination error: {ex.Message}";
            Debug.WriteLine($"UpdatePagedCars: Exception: {ex}");
        }
    }

    public bool HasNextPage => CurrentPage < TotalPages;
    public bool HasPreviousPage => CurrentPage > 1;

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < TotalPages)
            CurrentPage++;
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (CurrentPage > 1)
            CurrentPage--;
    }

    partial void OnCurrentPageChanged(int value)
    {
        UpdatePagedCars();
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(HasPreviousPage));
    }

    [ObservableProperty] private bool _carAddIsOpen = false;

    [ObservableProperty] private bool _carEditIsOpen = false;

    [ObservableProperty] private bool _isList = false;
    [ObservableProperty] private bool _isEmpty = true;
    [ObservableProperty] private bool _buttonIsVisible = false;

    [ObservableProperty]
    private string _errorText = "";

    [ObservableProperty]
    private string _messageText = "";

    [ObservableProperty]
    private string _listText = "";

    public bool IsAnyPopupOpen => CarAddIsOpen || CarEditIsOpen;

    [ObservableProperty] private string _vin = "";
    [ObservableProperty] private string _mark = "";
    [ObservableProperty] private string _model = "";
    [ObservableProperty] private string _productionYear = "";
    [ObservableProperty] private string _engineType = "";
    [ObservableProperty] private InsuranceViewModel? _selectedInsurance;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _itemsPerPage = 10;

    [ObservableProperty]
    private int _totalPages;

    [ObservableProperty]
    private ObservableCollection<CarViewModel> _pagedCars = new();

    partial void OnCarAddIsOpenChanged(bool value) => OnPropertyChanged(nameof(IsAnyPopupOpen));
    partial void OnCarEditIsOpenChanged(bool value) => OnPropertyChanged(nameof(IsAnyPopupOpen));

    [ObservableProperty]
    private CarViewModel _selectedCar = null!;

    public CarPageViewModel()
    {
        try
        {
            _ = InitializeAsync();
        }
        catch (Exception ex)
        {
            ErrorText = $"Constructor error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void CarAddOpen() {
        ErrorText = "";
        CarAddIsOpen ^= true;
    } 

    [RelayCommand]
    private async Task CarAddSave()
    {
        try
        {
            var user = AppState.LoggedInUser;
            if (user == null)
            {
                ErrorText = "No logged-in user.";
                return;
            }

            if (!int.TryParse(ProductionYear, out var productionYear))
            {
                ErrorText = "Production year must be a valid number.";
                return;
            }

            if (SelectedInsurance == null)
            {
                ErrorText = "Please select an insurance.";
                return;
            }

            var carInsurance = new CarInsurance
            {
                Id = Guid.NewGuid(),
                CarVIN = Vin,                  // the VIN of the car you're creating
                InsuranceTypeId = SelectedInsurance.ThisInsurance.Id,
                // InsuranceType = SelectedInsurance.ThisInsurance,
                ValidFrom = DateTime.UtcNow,      // set as appropriate
                ValidTo = DateTime.UtcNow.AddYears(1),  // example: valid for 1 year
                IsActive = true
            };

            var newCar = new Car
            {
                VIN = Vin,
                Mark = Mark,
                Model = Model,
                ProductionYear = productionYear,
                EngineType = EngineType,
                CarInsurances = new List<CarInsurance> { carInsurance }
            };

            var validator = new CarValidator();
            var result = await validator.ValidateAsync(newCar);
            if (result.IsValid)
            {
                var client = HttpClientFactory.CreateClient("CarInsuranceApi");
                Debug.WriteLine($"CarAddSave: Sending POST to api/Car?userId={user.Id}");
                var response = await client.PostAsJsonAsync($"api/Car?userId={user.Id}", newCar);
                if (!response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    ErrorText = $"{response.StatusCode}";
                    Vin = $"{json}";
                    return;
                }
                await LoadCarsAsync();
                CarAddIsOpen = false;
                ErrorText = "";
                MessageText = "Car added successfully.";
                ResetDefaultCar();
            }
            else
            {
                ErrorText = string.Join("\n", result.Errors.Select(e => $"- {e.ErrorMessage}"));
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorText = $"API error: {ex.StatusCode} - {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            ErrorText = $"Service error: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorText = $"Failed to add car: {ex.Message}";
        }
    }

    public void CarEditOpen(CarViewModel carViewModel, bool IsSuccess=false)
    {
        try
        {
            var car = carViewModel.Car;
            carViewModel.LoadFromCar(car);

            CarEditIsOpen ^= true;
            SelectedCar = carViewModel;
            if (IsSuccess)
                _ = LoadCarsAsync();
        }
        catch (Exception ex)
        {
            ErrorText = $"Failed to open edit: {ex.Message}";
        }
    }

    private async Task LoadCarsAsync()
    {
        try
        {
            var client = HttpClientFactory.CreateClient("CarInsuranceApi");

            var token = TokenStorage.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                ListText = "User is not logged in.";
                return;
            }

            ButtonIsVisible = true;
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/Car/user");

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ErrorText = $"API error: {response.StatusCode}";
                ListText = json.Trim('"');
                Debug.WriteLine($"LoadCarsAsync: API returned error - {json}");
                return;
            }

            var cars = await response.Content.ReadFromJsonAsync<Car[]>();
            if (!IsList)
            {
                IsList ^= true;
                IsEmpty ^= true;
            }

            if (cars == null)
            {
                ErrorText = "Failed to load cars from API.";
                MessageText = "Failed to load cars.";
                Debug.WriteLine("LoadCarsAsync: Cars array is null");
                return;
            }

            Cars = new ObservableCollection<CarViewModel>(
                cars.Select(car => new CarViewModel(car, this))
            );
            ApplyFilter();
        }
        catch (HttpRequestException ex)
        {
            ErrorText = $"API error: {ex.StatusCode} - {ex.Message}";
            MessageText = "Failed to load cars.";
        }
        catch (InvalidOperationException ex)
        {
            ErrorText = $"Service error: {ex.Message}";
            MessageText = "Failed to load cars.";
        }
        catch (Exception ex)
        {
            ErrorText = $"Failed to load cars: {ex.Message}";
            MessageText = "Failed to load cars.";
        }
    }

    private async Task LoadInsurancesAsync()
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
            var client = HttpClientFactory.CreateClient("CarInsuranceApi");

            var insurances = await client.GetFromJsonAsync<InsuranceType[]>($"api/InsuranceTypes/user/{user.Id}");
            if (insurances == null)
            {
                MessageText = "Failed to load insurances from API.";
                return;
            }

            Insurances = new ObservableCollection<InsuranceViewModel>(
                insurances.Select(ins => new InsuranceViewModel(ins))
            );

            if (!Insurances.Any())
            {
                MessageText = "Nuh uh";
                return;
            }
        }
        catch (HttpRequestException ex)
        {
            MessageText = $"API error: {ex.StatusCode} - {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            MessageText = $"Service error: {ex.Message}";
        }
        catch (Exception ex)
        {
            MessageText = $"Failed to load insurances: {ex.Message}";
        }
    }

    private void ResetDefaultCar()
    {
        try
        {
            Debug.WriteLine("ResetDefaultCar called");
            Vin = "";
            Mark = "";
            Model = "";
            ProductionYear = "";
            EngineType = "";
            SelectedInsurance = null;
        }
        catch (Exception ex)
        {
            ErrorText = $"Failed to reset car: {ex.Message}";
            Debug.WriteLine($"ResetDefaultCar: Exception: {ex}");
        }
    }

    private async Task InitializeAsync()
    {
        try
        {
            Debug.WriteLine("InitializeAsync started");
            AppState.OnLogin += async () =>
            {
                await LoadCarsAsync();
                await LoadInsurancesAsync();
            };

            AppState.OnLogOut += () =>
            {
                Cars.Clear();
                FilteredCars.Clear();
                PagedCars.Clear();
                IsList = false;
                IsEmpty = true;
                ButtonIsVisible = false;
                ListText = "You have been logged out.";
            };
            AppState.OnInsuranceChange += async () => await LoadInsurancesAsync();
            await LoadCarsAsync();
            await LoadInsurancesAsync();
        }
        catch (Exception ex)
        {
            ErrorText = $"Initialization failed: {ex.Message}";
        }
    }
}
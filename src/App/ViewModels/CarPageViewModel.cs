using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;
using Core.Validators;
using FluentValidation;

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

    private void ApplyFilter()
    {
        try
        {
            Debug.WriteLine("ApplyFilter called");
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                FilteredCars = new ObservableCollection<CarViewModel>(Cars);
                return;
            }

            var lower = FilterText.ToLowerInvariant();
            var result = Cars.Where(car =>
                (car.Car.Model?.ToLower().Contains(lower) ?? false) ||
                (car.Car.Mark?.ToLower().Contains(lower) ?? false) ||
                (car.Car.VIN?.ToLower().Contains(lower) ?? false)
            );
            FilteredCars = new ObservableCollection<CarViewModel>(result);
        }
        catch (Exception ex)
        {
            ErrorText = $"Failed to apply filter: {ex.Message}";
            Debug.WriteLine($"ApplyFilter error: {ex}");
        }
    }

    [ObservableProperty]
    private bool _carAddIsOpen = false;

    [ObservableProperty]
    private bool _carEditIsOpen = false;

    [ObservableProperty]
    private string _errorText = "";

    [ObservableProperty]
    private string _messageText = "";

    public bool IsAnyPopupOpen => CarAddIsOpen || CarEditIsOpen;

    [ObservableProperty] private string _vin = "";
    [ObservableProperty] private string _mark = "";
    [ObservableProperty] private string _model = "";
    [ObservableProperty] private string _productionYear = "";
    [ObservableProperty] private string _engineType = "";
    [ObservableProperty] private InsuranceViewModel? _selectedInsurance;

    partial void OnCarAddIsOpenChanged(bool value) => OnPropertyChanged(nameof(IsAnyPopupOpen));
    partial void OnCarEditIsOpenChanged(bool value) => OnPropertyChanged(nameof(IsAnyPopupOpen));

    [ObservableProperty]
    private Car? _selectedCar;

    public CarPageViewModel()
    {
        try
        {
            Debug.WriteLine("CarPageViewModel constructor started");
            _ = InitializeAsync();
            Debug.WriteLine("CarPageViewModel constructor completed");
        }
        catch (Exception ex)
        {
            ErrorText = $"Constructor error: {ex.Message}";
            Debug.WriteLine($"Constructor error: {ex}");
        }
    }

    [RelayCommand]
    private void CarAddOpen() => CarAddIsOpen ^= true;

    [RelayCommand]
    private async Task CarAddSave()
    {
        try
        {
            Debug.WriteLine("CarAddSave started");
            var user = AppState.LoggedInUser;
            if (user == null)
            {
                ErrorText = "No logged-in user.";
                Debug.WriteLine("CarAddSave: No logged-in user");
                return;
            }

            if (!int.TryParse(ProductionYear, out var productionYear))
            {
                ErrorText = "Production year must be a valid number.";
                Debug.WriteLine("CarAddSave: Invalid production year");
                return;
            }

            var newCar = new Car
            {
                VIN = Vin,
                Mark = Mark,
                Model = Model,
                ProductionYear = productionYear,
                EngineType = EngineType,
                InsuranceId = SelectedInsurance?.ThisInsurance.Id
            };

            var validator = new CarValidator();
            var result = await validator.ValidateAsync(newCar);
            if (result.IsValid)
            {
                var client = HttpClientFactory.CreateClient("CarInsuranceApi");
                Debug.WriteLine($"CarAddSave: Sending POST to api/Car?userId={user.Id}");
                var response = await client.PostAsJsonAsync($"api/Car?userId={user.Id}", newCar);
                response.EnsureSuccessStatusCode();
                await LoadCarsAsync();
                CarAddIsOpen = false;
                ErrorText = "";
                MessageText = "Car added successfully.";
                ResetDefaultCar();
                Debug.WriteLine("CarAddSave: Car added successfully");
            }
            else
            {
                ErrorText = string.Join("\n", result.Errors.Select(e => $"- {e.ErrorMessage}"));
                Debug.WriteLine($"CarAddSave: Validation failed: {ErrorText}");
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorText = $"API error: {ex.StatusCode} - {ex.Message}";
            Debug.WriteLine($"CarAddSave: HttpRequestException: {ex}");
        }
        catch (InvalidOperationException ex)
        {
            ErrorText = $"Service error: {ex.Message}";
            Debug.WriteLine($"CarAddSave: InvalidOperationException: {ex}");
        }
        catch (Exception ex)
        {
            ErrorText = $"Failed to add car: {ex.Message}";
            Debug.WriteLine($"CarAddSave: Exception: {ex}");
        }
    }

    public void CarEditOpen(Car car)
    {
        try
        {
            Debug.WriteLine($"CarEditOpen: VIN={car?.VIN}");
            CarEditIsOpen ^= true;
            SelectedCar = car;
        }
        catch (Exception ex)
        {
            ErrorText = $"Failed to open edit: {ex.Message}";
            Debug.WriteLine($"CarEditOpen: Exception: {ex}");
        }
    }

    private async Task TestGetAllCarsAsync()
    {
        try
        {
            Debug.WriteLine("TestGetAllCarsAsync started");
            if (AppState.ServiceProvider == null)
            {
                ErrorText = "ServiceProvider not initialized.";
                MessageText = "Failed to load cars. 1";
                Debug.WriteLine("TestGetAllCarsAsync: ServiceProvider is null");
                return;
            }

            var client = HttpClientFactory.CreateClient("CarInsuranceApi");
            Debug.WriteLine("TestGetAllCarsAsync: Sending GET to api/Car");
            var response = await client.GetAsync("api/Car");
            Debug.WriteLine($"TestGetAllCarsAsync: HTTP Status: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorText = $"API error: {response.StatusCode} - {errorContent}";
                MessageText = "Failed to load cars. 2";
                Debug.WriteLine($"TestGetAllCarsAsync: Failed with status {response.StatusCode}, content: {errorContent}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"TestGetAllCarsAsync: Response JSON: {json}");
            var cars = await response.Content.ReadFromJsonAsync<Car[]>();

            if (cars == null)
            {
                ErrorText = "Failed to deserialize cars from API response.";
                MessageText = "Failed to load cars. 3";
                Debug.WriteLine("TestGetAllCarsAsync: Cars array is null after deserialization");
                return;
            }

            Cars = new ObservableCollection<CarViewModel>(
                cars.Select(car => new CarViewModel(car, this))
            );
            ApplyFilter();
            MessageText = cars.Any() ? $"{cars.Length} cars loaded." : "No cars found.";
            Debug.WriteLine($"TestGetAllCarsAsync: Loaded {cars.Length} cars");
        }
        catch (HttpRequestException ex)
        {
            ErrorText = $"Network error: {ex.Message}";
            MessageText = "Failed to load cars. 4";
            Debug.WriteLine($"TestGetAllCarsAsync: HttpRequestException: {ex.Message}, Inner: {ex.InnerException?.Message}");
        }
        catch (InvalidOperationException ex)
        {
            ErrorText = $"Service error: {ex.Message}";
            MessageText = "Failed to load cars. 5";
            Debug.WriteLine($"TestGetAllCarsAsync: InvalidOperationException: {ex.Message}");
        }
        catch (Exception ex)
        {
            ErrorText = $"Unexpected error: {ex.Message}";
            MessageText = "Failed to load cars. 6";
            Debug.WriteLine($"TestGetAllCarsAsync: Exception: {ex.Message}, StackTrace: {ex.StackTrace}");
        }
    }
    private async Task LoadCarsAsync()
    {
        try
        {
            Debug.WriteLine("LoadCarsAsync started");
            var client = HttpClientFactory.CreateClient("CarInsuranceApi");
            Car[]? cars;
            var user = AppState.LoggedInUser;
            Debug.WriteLine($"LoadCarsAsync: User ID={user?.Id}");
            if (user != null)
            {
                Debug.WriteLine($"LoadCarsAsync: Fetching api/Car/user/{user.Id}");
                var response = await client.GetAsync($"api/Car/user/{user.Id}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"LoadCarsAsync: Response JSON={json}");
                cars = await response.Content.ReadFromJsonAsync<Car[]>();
            }
            else
            {
                Debug.WriteLine("LoadCarsAsync: Fetching api/Car (no user)");
                cars = await client.GetFromJsonAsync<Car[]>("api/Car");
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
            MessageText = cars.Any() ? $"{cars.Length} cars loaded." : "No cars found.";
            Debug.WriteLine($"LoadCarsAsync: Loaded {cars.Length} cars");
        }
        catch (HttpRequestException ex)
        {
            ErrorText = $"API error: {ex.StatusCode} - {ex.Message}";
            MessageText = "Failed to load cars.";
            Debug.WriteLine($"LoadCarsAsync: HttpRequestException: {ex}");
        }
        catch (InvalidOperationException ex)
        {
            ErrorText = $"Service error: {ex.Message}";
            MessageText = "Failed to load cars.";
            Debug.WriteLine($"LoadCarsAsync: InvalidOperationException: {ex}");
        }
        catch (Exception ex)
        {
            ErrorText = $"Failed to load cars: {ex.Message}";
            MessageText = "Failed to load cars.";
            Debug.WriteLine($"LoadCarsAsync: Exception: {ex}");
        }
    }

    private async Task LoadInsurancesAsync()
    {
        try
        {
            Debug.WriteLine("LoadInsurancesAsync started");
            var client = HttpClientFactory.CreateClient("CarInsuranceApi");
            Debug.WriteLine("LoadInsurancesAsync: Fetching api/Insurance");
            var insurances = await client.GetFromJsonAsync<Insurance[]>("api/Insurance");
            if (insurances == null)
            {
                ErrorText = "Failed to load insurances from API.";
                Debug.WriteLine("LoadInsurancesAsync: Insurances array is null");
                return;
            }

            Insurances = new ObservableCollection<InsuranceViewModel>(
                insurances.Select(ins => new InsuranceViewModel(ins))
            );
            Debug.WriteLine($"LoadInsurancesAsync: Loaded {insurances.Length} insurances");
        }
        catch (HttpRequestException ex)
        {
            ErrorText = $"API error: {ex.StatusCode} - {ex.Message}";
            Debug.WriteLine($"LoadInsurancesAsync: HttpRequestException: {ex}");
        }
        catch (InvalidOperationException ex)
        {
            ErrorText = $"Service error: {ex.Message}";
            Debug.WriteLine($"LoadInsurancesAsync: InvalidOperationException: {ex}");
        }
        catch (Exception ex)
        {
            ErrorText = $"Failed to load insurances: {ex.Message}";
            Debug.WriteLine($"LoadInsurancesAsync: Exception: {ex}");
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
            await TestGetAllCarsAsync(); // Test API call
            // MessageText = "TestGetAllCarsAsync completed.";
        }
        catch (Exception ex)
        {
            ErrorText = $"Initialization failed: {ex.Message}";
            Debug.WriteLine($"InitializeAsync: Exception: {ex}");
        }
    }
}
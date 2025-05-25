namespace App.ViewModels.CarPageViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;

public partial class CarViewModel : ViewModelBase
{
    [ObservableProperty] private string _vin = "";
    [ObservableProperty] private string _mark = "";
    [ObservableProperty] private string _model = "";
    [ObservableProperty] private string _productionYear = "";
    [ObservableProperty] private string _engineType = "";

    [ObservableProperty] private string _errorText = "";

    [ObservableProperty] private bool _hasActiveInsurance;
    [ObservableProperty] private int? _daysUntilExpiration;

    [ObservableProperty] private ObservableCollection<InsuranceViewModel> _availableInsuranceTypes = new();
    [ObservableProperty] private InsuranceViewModel? _selectedInsuranceType;

    public Car Car { get; private set; }

    public void LoadFromCar(Car car)
    {
        Car = car;

        Vin = car.VIN;
        Mark = car.Mark;
        Model = car.Model;
        ProductionYear = car.ProductionYear.ToString();
        EngineType = car.EngineType;

        var activeInsurance = car.CarInsurances?.FirstOrDefault(i => i.IsActive);

        if (activeInsurance?.InsuranceType != null)
        {
            // Patch missing firm from preloaded insurance list
            var fullInsurance = _availableInsuranceTypes.FirstOrDefault(vm =>
                vm.ThisInsurance.Id == activeInsurance.InsuranceType.Id);

            if (fullInsurance != null)
            {
                SelectedInsuranceType = fullInsurance;
            }
            else
            {
                // fallback with possibly incomplete insurance
                SelectedInsuranceType = new InsuranceViewModel(activeInsurance.InsuranceType);
            }
        }

        HasActiveInsurance = activeInsurance != null;
        DaysUntilExpiration = activeInsurance != null
            ? (activeInsurance.ValidTo - DateTime.Now).Days
            : null;
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        if (Car == null) return;

        if (!int.TryParse(ProductionYear, out var parsedYear))
            return;

        Car.VIN = Vin;
        Car.Mark = Mark;
        Car.Model = Model;
        Car.ProductionYear = parsedYear;
        Car.EngineType = EngineType;

        if (SelectedInsuranceType != null)
        {
            var existing = Car.CarInsurances.FirstOrDefault(i => i.IsActive);
            if (existing != null)
            {
                existing.InsuranceTypeId = SelectedInsuranceType.ThisInsurance.Id;
                existing.ValidTo = DateTime.Now.AddYears(1);
            }
            else
            {
                Car.CarInsurances.Add(new CarInsurance
                {
                    CarVIN = Vin,
                    InsuranceTypeId = SelectedInsuranceType.ThisInsurance.Id,
                    ValidFrom = DateTime.Now,
                    ValidTo = DateTime.Now.AddYears(1),
                    IsActive = true
                });
            }
        }
    }

    private readonly CarPageViewModel _carPageViewModel;

    public CarViewModel(Car car, CarPageViewModel carPageViewModel)
    {
        Car = car;
        _ = LoadInsurancesAsync();
        _carPageViewModel = carPageViewModel;
    }

    [RelayCommand]
    private void ShowEdit()
    {
        _carPageViewModel.CarEditOpen(this);
    }

    public string DaysUntilNearestInsuranceExpiresText
    {
        get
        {
            var upcoming = Car.CarInsurances?
                .Where(ci => ci.ValidTo > DateTime.Now)
                .OrderBy(ci => ci.ValidTo)
                .FirstOrDefault();

            return upcoming != null
                ? $"{(upcoming.ValidTo - DateTime.Now).Days} days"
                : "n/a";
        }
    }

    public string HasActiveInsuranceText =>
        Car.CarInsurances?.Any(ci => ci.IsActive && ci.ValidTo > DateTime.Now) == true
            ? "Yes"
            : "n/a";

    private async Task LoadInsurancesAsync()
    {
        try
        {
            var client = HttpClientFactory.CreateClient("CarInsuranceApi");

            var insurances = await client.GetFromJsonAsync<InsuranceType[]>("api/InsuranceType");
            if (insurances == null)
            {
                ErrorText = "Failed to load insurances from API.";
                return;
            }

            AvailableInsuranceTypes = new ObservableCollection<InsuranceViewModel>(
                insurances.Select(ins => new InsuranceViewModel(ins))
            );
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
            ErrorText = $"Failed to load insurances: {ex.Message}";
        }
    }
}
namespace App.ViewModels.CarPageViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
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

    [ObservableProperty] private bool _hasActiveInsurance;
    [ObservableProperty] private int? _daysUntilExpiration;

    [ObservableProperty] private ObservableCollection<InsuranceType> _availableInsuranceTypes = new();
    [ObservableProperty] private InsuranceType? _selectedInsuranceType;

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
        HasActiveInsurance = activeInsurance != null;
        DaysUntilExpiration = activeInsurance != null ? (activeInsurance.ValidTo - DateTime.Now).Days : null;
        SelectedInsuranceType = activeInsurance?.InsuranceType;
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
                existing.InsuranceTypeId = SelectedInsuranceType.Id;
                existing.ValidTo = DateTime.Now.AddYears(1);
            }
            else
            {
                Car.CarInsurances.Add(new CarInsurance
                {
                    CarVIN = Vin,
                    InsuranceTypeId = SelectedInsuranceType.Id,
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
}
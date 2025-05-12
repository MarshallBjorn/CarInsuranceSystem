using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Core.Entities;
using Infrastructure;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using Core.Validators;
using System;

namespace App.ViewModels;

public partial class CarPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public ObservableCollection<CarViewModel> _cars = new();

    [ObservableProperty]
    private ObservableCollection<CarViewModel> _filteredCars = new();
    
    [ObservableProperty]
    public ObservableCollection<InsuranceViewModel>? insurances;

    [ObservableProperty]
    private string _filterText = "";

    partial void OnFilterTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            FilteredCars = new ObservableCollection<CarViewModel>(Cars);
            return;
        }

        var lower = FilterText.ToLowerInvariant();

        var result = Cars.Where(car =>
            car.Car.Model.ToLower().Contains(lower) ||
            car.Car.Mark.ToLower().Contains(lower) ||
            car.Car.VIN.ToLower().Contains(lower)
        );

        FilteredCars = new ObservableCollection<CarViewModel>(result);
    }


    [ObservableProperty]
    private bool _carAddIsOpen = false;

    [ObservableProperty]
    private bool _carEditIsOpen = false;

    [ObservableProperty]
    private string _ErrorText = "";

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
        _ = InitializeAsync();
    }

    // Command which opens add form popup. Works directly from here.
    [RelayCommand]
    private void CarAddOpen() => CarAddIsOpen ^= true;

    [RelayCommand]
    private async Task CarAddSave() {
        try {
            var user = ServiceLocator.AppState.LoggedInUser;
            var insurance = SelectedInsurance?.ThisInsurance;

            Car newCar = new() {
                VIN = Vin,
                Mark = Mark,
                Model = Model,
                ProductionYear = Int32.Parse(ProductionYear),
                EngineType = EngineType,
                InsuranceId = insurance?.Id,
                Insurance = insurance
            };

            var validator = new CarValidator();
            var result = validator.Validate(newCar);
            

            if (result.IsValid && user is not null)
            {
                await ServiceLocator.CarService.AddCarAsync(newCar, user);
                _ = LoadCarsAsync();
                CarAddIsOpen ^= true;
                ErrorText = "";
                ResetDefaultCar();
            } else {
                string ErrorMessages = string.Join("\n", result.Errors.Select(e => $"- {e.ErrorMessage}"));
                ErrorText = ErrorMessages;
            }
        } catch (Exception ex)
        {
            ErrorText = ex.Message;
        }
    }

    // Command which helps edit form popup. It alowes to transfer operational data.
    public void CarEditOpen(Car car)
    {
        CarEditIsOpen ^= true;
        SelectedCar = car;
    }

    private async Task LoadCarsAsync()
    {
        var user = ServiceLocator.AppState.LoggedInUser;
        if (user is null) {
            MessageText = "No logged in user";
        } else {
            var cars = await ServiceLocator.CarService.GetCarsUserAsync(user);
        
            Cars = new ObservableCollection<CarViewModel>(
                cars.Select(car => new CarViewModel(car, this))
            );
            ApplyFilter();
        }
    }

    private async Task LoadInsurancesAsync()
    {
        var insurances = await ServiceLocator.InsuranceService.GetInsurancesAsync();
        Insurances = new ObservableCollection<InsuranceViewModel>(
            insurances.Select(ins => new InsuranceViewModel(ins))
        );
    }

    private void ResetDefaultCar()
    {
        Vin = "";
        Mark = "";
        Model = "";
        ProductionYear = "";
        EngineType = "";
    }

    private async Task InitializeAsync()
    {
        await LoadInsurancesAsync();
        await LoadCarsAsync();

        var user = ServiceLocator.AppState.LoggedInUser;
        if (user is not null) {
            MessageText = $"Welcome {user.FirstName} {user.LastName}";
        }
    }
}

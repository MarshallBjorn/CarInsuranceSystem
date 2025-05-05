using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Core.Entities;
using Infrastructure;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using Core.Validators;
using System.IO;

namespace App.ViewModels;

public partial class CarPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public ObservableCollection<CarViewModel>? cars;

    [ObservableProperty]
    private bool _carAddIsOpen = false;

    [ObservableProperty]
    private bool _carEditIsOpen = false;

    [ObservableProperty]
    private string _ErrorText = "";
    
    public bool IsAnyPopupOpen => CarAddIsOpen || CarEditIsOpen;

    [ObservableProperty] private string _vin = "";
    [ObservableProperty] private string _mark = "";
    [ObservableProperty] private string _model = "";
    [ObservableProperty] private int _productionYear = 0;
    [ObservableProperty] private string _engineType = "";

    partial void OnCarAddIsOpenChanged(bool value) => OnPropertyChanged(nameof(IsAnyPopupOpen));
    partial void OnCarEditIsOpenChanged(bool value) => OnPropertyChanged(nameof(IsAnyPopupOpen));

    [ObservableProperty]
    private Car? _selectedCar;

    public CarPageViewModel()
    {
        _ = LoadCarsAsync();
    }

    // Command which opens add form popup. Works directly from here.
    [RelayCommand]
    private void CarAddOpen() => CarAddIsOpen ^= true;

    [RelayCommand]
    private async Task CarAddSave() {
        Car newCar = new() {
            VIN = Vin,
            Mark = Mark,
            Model = Model,
            ProductionYear = ProductionYear,
            EngineType = EngineType,
        };

        var validator = new CarValidator();
        var result = validator.Validate(newCar);

        if (!result.IsValid)
        {
            string ErrorMessages = string.Join("\n", result.Errors.Select(e => $"- {e.ErrorMessage}"));
            ErrorText = ErrorMessages;
        } else {
            await ServiceLocator.CarService.AddCarAsync(newCar);
            _ = LoadCarsAsync();
            CarAddIsOpen ^= true;
            ErrorText = "";
            ResetDefaultCar();
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
        var cars = await ServiceLocator.CarService.GetCarsAsync();
        
        Cars = new ObservableCollection<CarViewModel>(
            cars.Select(car => new CarViewModel(car, this))
        );
    }

    private void ResetDefaultCar()
    {
        Vin = "";
        Mark = "";
        Model = "";
        ProductionYear = 0;
        EngineType = "";
    }
}
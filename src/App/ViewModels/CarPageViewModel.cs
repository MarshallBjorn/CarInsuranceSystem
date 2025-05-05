using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Core.Entities;
using Infrastructure;
using App.Views;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace App.ViewModels;

public partial class CarPageViewModel : ViewModelBase
{
    public ObservableCollection<Car> Cars { get; set; } = new ObservableCollection<Car>();

    public CarPageViewModel()
    {
        _ = LoadCarsAsync();
    }

    private async Task LoadCarsAsync()
    {
        var cars = await ServiceLocator.CarService.GetCarsAsync();
        Cars.Clear();
        foreach (var car in cars)
            Cars.Add(car);
    }

    [ObservableProperty]
    private bool _carAddIsOpen = false;

    [ObservableProperty]
    private bool _carEditIsOpen = false;
    
    [ObservableProperty]
    private Car? _selectedCar;

    [RelayCommand]
    private void CarAdd() => CarAddIsOpen ^= true;

    [RelayCommand]
    private void CarEdit()
    {
        CarEditIsOpen ^= true;
    }
}
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Core.Entities;
using Infrastructure;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;

namespace App.ViewModels;

public partial class CarPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public ObservableCollection<CarViewModel>? cars;

    [ObservableProperty]
    private bool _carAddIsOpen = false;

    [ObservableProperty]
    private bool _carEditIsOpen = false;
    
    public bool IsAnyPopupOpen => CarAddIsOpen || CarEditIsOpen;

    partial void OnCarAddIsOpenChanged(bool value) => OnPropertyChanged(nameof(IsAnyPopupOpen));
    partial void OnCarEditIsOpenChanged(bool value) => OnPropertyChanged(nameof(IsAnyPopupOpen));

    [ObservableProperty]
    private Car? _selectedCar;

    public CarPageViewModel()
    {
        _ = LoadCarsAsync();
    }

    private async Task LoadCarsAsync()
    {
        var cars = await ServiceLocator.CarService.GetCarsAsync();
        
        Cars = new ObservableCollection<CarViewModel>(
            cars.Select(car => new CarViewModel(car, this))
        );
    }

    // Command which opens add form popup. Works directly from here.
    [RelayCommand]
    private void CarAdd() => CarAddIsOpen ^= true;

    // Command which helps edit form popup. It alowes to transfer operational data.
    public void ShowCarEdit(Car car)
    {
        CarEditIsOpen ^= true;
        SelectedCar = car;
    }
}
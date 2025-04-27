using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Entities;
using Infrastructure;
using Infrastructure.Services;

namespace App.ViewModels;

public partial class CarPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _errorText;

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
}
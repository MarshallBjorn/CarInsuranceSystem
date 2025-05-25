namespace App.ViewModels.CarPageViewModels;

using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entities;

public partial class CarViewModel : ViewModelBase
{
    public Car Car { get; }

    private readonly CarPageViewModel _carPageViewModel;

    public CarViewModel(Car car, CarPageViewModel carPageViewModel)
    {
        Car = car;
        _carPageViewModel = carPageViewModel;
    }

    [RelayCommand]
    private void ShowEdit()
    {
        _carPageViewModel.CarEditOpen(Car);
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
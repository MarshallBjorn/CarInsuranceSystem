namespace App.ViewModels.CarPageViewModels;

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
}
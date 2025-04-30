namespace App.ViewModels;

using Core.Entities;

public class CarEditViewModel : ViewModelBase
{
    public Car EditableCar { get; set; }

    public CarEditViewModel(Car car)
    {
        EditableCar = new Car
        {
            VIN = car.VIN,
            Mark = car.Mark,
            Model = car.Model
            // Copy all fields you need
        };
    }
}

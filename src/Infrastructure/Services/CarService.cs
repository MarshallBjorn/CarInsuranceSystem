using Core.Entities;
using FluentValidation;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class CarService
{
    private readonly ICarRepository _repository;
    private readonly IValidator<Car> _carValidator;

    public CarService(ICarRepository repository, IValidator<Car> carValidator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _carValidator = carValidator ?? throw new ArgumentNullException(nameof(carValidator));
    }

    public Task<List<Car>> GetCarsAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<List<Car>> GetCarsUserAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        return _repository.GetCarsUserAsync(user);
    }

    public async Task<Car> GetByVINAsync(string vin)
    {
        return await _repository.GetByVINAsync(vin);
    }

    public async Task<bool> AddCarAsync(Car car, User user)
    {
        if (car == null)
            throw new ArgumentNullException(nameof(car));
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var validationResult = await _carValidator.ValidateAsync(car);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return await _repository.AddCarAsync(car, user);
    }
    
    public async Task<bool> UpdateCarDataAsync(string vin, Car updatedCar)
    {
        if (string.IsNullOrWhiteSpace(vin))
            throw new ArgumentException("VIN cannot be null or empty.", nameof(vin));

        if (updatedCar == null)
            throw new ArgumentNullException(nameof(updatedCar));

        var validationResult = await _carValidator.ValidateAsync(updatedCar);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return await _repository.UpdateCarDataAsync(vin, updatedCar);
    }
}
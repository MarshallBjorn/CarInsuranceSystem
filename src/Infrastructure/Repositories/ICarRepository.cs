using Core.Entities;

namespace Infrastructure.Repositories;

public interface ICarRepository
{
    Task<List<Car>> GetAllAsync();
    Task<List<Car>> GetCarsUserAsync(User user);
    Task<Car> GetByVINAsync(String VIN);
    Task<bool> UpdateCarDataAsync(String VIN, Car updatedCar);
    Task<bool> AddCarAsync(Car car, User user, Insurance? insurance);
}
using Core.Entities;

namespace Infrastructure.Repositories;

public interface ICarRepository
{
    Task<List<Car>> GetAllAsync();
    Task<Car> GetByVINAsync(String VIN);
}
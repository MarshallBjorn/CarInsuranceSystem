using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Services
{
    public class CarService
    {
        private readonly CarRepository _repository;

        public CarService(CarRepository repository) => _repository = repository;

        public Task<List<Car>> GetCarsAsync() => _repository.GetAllAsync();

        public Task<List<Car>> GetCarsUserAsync(User user) => _repository.GetCarsUserAsync(user);

        public Task<bool> AddCarAsync(Car car, User user, Insurance? insurance) => _repository.AddCarAsync(car, user, insurance);
    }
}

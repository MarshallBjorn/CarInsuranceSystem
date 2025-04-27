using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Services
{
    public class CarService
    {
        private readonly CarRepository _repository;

        public CarService(CarRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Car>> GetCarsAsync() => _repository.GetAllAsync();
    }
}

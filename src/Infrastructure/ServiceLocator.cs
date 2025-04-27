using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace Infrastructure
{
    public static class ServiceLocator
    {
        private static AppDbContext _dbContext;
        private static CarService _carService;
        
        static ServiceLocator()
        {
            var dbContextFactory = new AppDbContextFactory();
            _dbContext = dbContextFactory.CreateDbContext();

            var carRepository = new CarRepository(_dbContext);
            _carService = new CarService(carRepository);
        }

        public static CarService CarService => _carService;
    }
}

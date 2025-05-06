using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.State;

namespace Infrastructure
{
    public static class ServiceLocator
    {
        private static AppDbContext _dbContext;
        private static CarService _carService;
        private static UserService _userService;

        private static AppState _appState = new AppState();

        static ServiceLocator()
        {
            var dbContextFactory = new AppDbContextFactory();
            _dbContext = dbContextFactory.CreateDbContext();

            var carRepository = new CarRepository(_dbContext);
            _carService = new CarService(carRepository);

            var userRepository = new UserRepository(_dbContext);
            _userService = new UserService(userRepository); 
        }

        public static CarService CarService => _carService;

        public static UserService UserService => _userService;
        
        public static AppState AppState => _appState;
    }
}

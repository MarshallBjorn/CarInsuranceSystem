using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CarRepository(AppDbContext context) : ICarRepository
{
    private readonly AppDbContext _context = context;

    public async Task<List<Car>> GetAllAsync()
    {
        return await _context.Cars.ToListAsync();
    }

    public async Task<Car> GetByVINAsync(string VIN)
    {
        var car = await _context.Cars.FindAsync(VIN);
        return car ?? throw new InvalidOperationException("Car with this VIN has not been found.");
    }

    // TODO
    public Task<bool> UpdateCarDataAsync(String VIN, Car updatedCar)
    {
        throw new NotImplementedException();
    }

    // TODO
    public async Task<bool> AddCarAsync(Car car, User user, Insurance? insurance = null)
    {
        try
        {
            // If insurance is provided, add it to the context first
            if (insurance != null)
            {
                _context.Insurances.Add(insurance);
                car.InsuranceId = insurance.Id; // Link the car to the insurance
            }

            var userCar = new UserCar
            {
                UserId = user.Id,
                CarVIN = car.VIN,
                User = user,
                Car = car,
                PurchaseDate = DateTime.UtcNow,
                IsCurrentOwner = true
            };

            _context.Cars.Add(car);
            _context.UserCars.Add(userCar);
            await _context.SaveChangesAsync();
            return true;
        } 
        catch
        {
            return false;
        }
    }

    public async Task<List<Car>> GetCarsUserAsync(User user)
    {
        var cars = await _context.UserCars
            .Where(uc => uc.UserId == user.Id)
            .Include(uc => uc.Car)
            .Select(uc => uc.Car!)
            .ToListAsync();
        if (!cars.Any()) 
            throw new Exception("You have no cars registered.");
        return cars;
    }
}
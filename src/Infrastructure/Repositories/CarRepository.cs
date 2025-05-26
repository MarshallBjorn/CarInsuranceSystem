using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories;

public class CarRepository : ICarRepository
{
    private readonly AppDbContext _context;

    public CarRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<Car>> GetAllAsync()
    {
        return await _context.Cars.ToListAsync();
    }

    public async Task<Car> GetByVINAsync(string vin)
    {
        if (string.IsNullOrWhiteSpace(vin))
            throw new ArgumentException("VIN cannot be null or empty.", nameof(vin));

        var car = await _context.Cars.FindAsync(vin);
        return car ?? throw new KeyNotFoundException($"Car with VIN {vin} not found.");
    }

    public async Task<bool> UpdateCarDataAsync(string vin, Car updatedCar)
    {
        if (string.IsNullOrWhiteSpace(vin))
            throw new ArgumentException("VIN cannot be null or empty.", nameof(vin));

        if (updatedCar == null)
            throw new ArgumentNullException(nameof(updatedCar));

        var existingCar = await _context.Cars.FindAsync(vin);
        if (existingCar == null)
            return false;

        // Update only mutable fields (VIN stays as primary key)
        existingCar.Mark = updatedCar.Mark;
        existingCar.Model = updatedCar.Model;
        existingCar.ProductionYear = updatedCar.ProductionYear;
        existingCar.EngineType = updatedCar.EngineType;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            // Optionally log exception
            throw new InvalidOperationException("Failed to update car data.", ex);
        }
    }

    public async Task<bool> AddCarAsync(Car car, User user)
    {
        if (car == null)
            throw new ArgumentNullException(nameof(car));
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var existingCar = await _context.Cars
            .Include(c => c.CarInsurances)
                .ThenInclude(ci => ci.InsuranceType)
                    .ThenInclude(it => it.Firm) // optional: if you want the whole chain
            .FirstOrDefaultAsync(c => c.VIN == car.VIN);

        if (existingCar != null)
        {
            throw new Exception("This car already exists");
        }
        
        _context.Users.Attach(user);

        var userCar = new UserCar
        {
            UserId = user.Id,
            CarVIN = car.VIN,
            Car = car,
            User = user,
            PurchaseDate = DateTime.UtcNow,
            IsCurrentOwner = true
        };

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Cars.Add(car);
            _context.UserCars.Add(userCar);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            // Log ex
            throw new InvalidOperationException("Failed to add car.", ex);
        }
    }

    public async Task<List<Car>> GetCarsUserAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var cars = await _context.UserCars
            .Where(uc => uc.UserId == user.Id)
            .Include(uc => uc.Car)
                .ThenInclude(c => c.CarInsurances)
                    .ThenInclude(ci => ci.InsuranceType)
            .Select(uc => uc.Car!)
            .ToListAsync();

        if (!cars.Any())
            throw new KeyNotFoundException($"No cars registered for user with email {user.Email}.");

        return cars;
    }
}
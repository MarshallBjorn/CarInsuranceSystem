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
    public Task<bool> AddCarAsync(Car car)
    {
        throw new NotImplementedException();
    }

    // TODO
    public Task<List<Car>> GetCarsUserAsync(int id)
    {
        throw new NotImplementedException();
    }
}
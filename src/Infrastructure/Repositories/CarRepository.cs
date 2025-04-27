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
        return await _context.Cars.FindAsync(VIN);
    }
}
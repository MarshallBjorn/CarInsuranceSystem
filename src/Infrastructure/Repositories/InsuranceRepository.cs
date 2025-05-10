using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InsuranceRepository(AppDbContext context) : IInsuranceRepository
{
    private readonly AppDbContext _context = context;

    public async Task<List<Insurance>> GetInsurancesAsync()
    {
        return await _context.Insurances
            .Include(i => i.Firm)
            .ToListAsync();
    }
}
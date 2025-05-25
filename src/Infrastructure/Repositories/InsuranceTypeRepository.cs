using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InsuranceTypeRepository : IInsuranceTypeRepository
{
    private readonly AppDbContext _context;

    public InsuranceTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InsuranceType>> GetAllAsync()
    {
        return await _context.InsuranceTypes
            .Include(f => f.Firm)
            .ToListAsync();
    }
}

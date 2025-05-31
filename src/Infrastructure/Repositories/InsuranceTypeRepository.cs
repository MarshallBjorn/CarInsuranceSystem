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

    public async Task<List<InsuranceType>> GetAllWithFirmAsync()
    {
        return await _context.InsuranceTypes.Include(i => i.Firm).ToListAsync();
    }

    public async Task<InsuranceType?> GetByIdAsync(Guid id)
    {
        return await _context.InsuranceTypes.Include(i => i.Firm).FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<InsuranceType> AddAsync(InsuranceType entity)
    {
        _context.InsuranceTypes.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(InsuranceType entity)
    {
        _context.InsuranceTypes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.InsuranceTypes.FindAsync(id);
        if (entity != null)
        {
            _context.InsuranceTypes.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<InsuranceType>> GetByUserIdAsync(Guid userId)
    {
        return await _context.InsuranceTypes
            .Include(i => i.Firm)
            .Where(i => i.Firm.UserId == userId)
            .ToListAsync();
    }

    public async Task<int> GetActiveCountAsync()
    {
        return await _context.CarInsurances
            .Where(i => i.IsActive)
            .CountAsync();
    }
}

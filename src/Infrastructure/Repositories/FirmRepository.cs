using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FirmRepository : IFirmRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public FirmRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Firm>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Firms.Include(f => f.InsuranceTypes).ToListAsync();
    }

    public async Task<Firm?> GetByIdAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Firms.Include(f => f.InsuranceTypes)
                                  .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task CreateAsync(Firm firm)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Firms.Add(firm);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Firm firm)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Firms.Update(firm);
        await context.SaveChangesAsync();
    }
}

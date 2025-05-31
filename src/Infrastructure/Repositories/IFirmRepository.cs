namespace Infrastructure.Repositories;
using Core.Entities;

public interface IFirmRepository
{
    Task<List<Firm>> GetAllAsync();
    Task<Firm?> GetByIdAsync(Guid id);
    Task CreateAsync(Firm firm);
    Task UpdateAsync(Firm firm);
    Task<List<Firm>> GetAllByUserIdAsync(Guid userId);
    Task<int> GetCountAsync();
}

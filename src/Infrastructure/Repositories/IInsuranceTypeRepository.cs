namespace Infrastructure.Repositories;

public interface IInsuranceTypeRepository
{
    Task<List<InsuranceType>> GetAllWithFirmAsync();
    Task<InsuranceType?> GetByIdAsync(Guid id);
    Task<InsuranceType> AddAsync(InsuranceType entity);
    Task UpdateAsync(InsuranceType entity);
    Task DeleteAsync(Guid id);
    Task<List<InsuranceType>> GetByUserIdAsync(Guid userId);
    Task<int> GetActiveCountAsync();
}

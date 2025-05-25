namespace Infrastructure.Repositories;

public interface IInsuranceTypeRepository
{
    Task<IEnumerable<InsuranceType>> GetAllAsync();
}

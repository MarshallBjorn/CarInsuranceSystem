using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class InsuranceTypeService
{
    private readonly IInsuranceTypeRepository _repository;

    public InsuranceTypeService(IInsuranceTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<InsuranceType>> GetAllInsuranceTypesAsync()
    {
        return await _repository.GetAllAsync();
    }
}
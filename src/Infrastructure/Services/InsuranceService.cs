using Core.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class InsuranceService
{
    private readonly InsuranceRepository _repository;

    public InsuranceService(InsuranceRepository repository) => _repository = repository;

    public Task<List<Insurance>> GetInsurancesAsync() => _repository.GetInsurancesAsync();
}
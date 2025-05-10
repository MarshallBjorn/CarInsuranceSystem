using Core.Entities;

namespace Infrastructure.Repositories;

public interface IInsuranceRepository
{
    Task<List<Insurance>> GetInsurancesAsync();
}
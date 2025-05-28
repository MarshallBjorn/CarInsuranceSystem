using Core.DTOs;

namespace Infrastructure.Services;

public interface IInsuranceTypeService
{
    Task<List<InsuranceTypeDto>> GetAllAsync();
    Task<InsuranceTypeDto?> GetByIdAsync(Guid id);
    Task<InsuranceTypeDto> CreateAsync(CreateUpdateInsuranceTypeDto dto);
    Task UpdateAsync(Guid id, CreateUpdateInsuranceTypeDto dto);
    Task DeleteAsync(Guid id);
}


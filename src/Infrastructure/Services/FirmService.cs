using Core.DTOs;
using Core.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class FirmService : IFirmService
{
    private readonly IFirmRepository _repository;

    public FirmService(IFirmRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<FirmDto>> GetAllAsync()
    {
        var firms = await _repository.GetAllAsync();

        return firms.Select(f => new FirmDto
        {
            Id = f.Id,
            Name = f.Name,
            CountryCode = f.CountryCode,
            CreatedAt = f.CreatedAt,
            InsuranceTypes = f.InsuranceTypes.Select(it => new InsuranceTypeDto
            {
                Id = it.Id,
                Name = it.Name,
                PolicyDescription = it.PolicyDescription,
                PolicyNumber = it.PolicyNumber
            }).ToList()
        }).ToList();
    }

    public async Task CreateAsync(CreateFirmDto dto)
    {
        var firm = new Firm
        {
            Name = dto.Name,
            CountryCode = dto.CountryCode
        };

        await _repository.CreateAsync(firm);
    }

    public async Task UpdateAsync(UpdateFirmDto dto)
    {
        var firm = await _repository.GetByIdAsync(dto.Id);
        if (firm == null) throw new KeyNotFoundException("Firm not found.");

        firm.Name = dto.Name;
        firm.CountryCode = dto.CountryCode;

        await _repository.UpdateAsync(firm);
    }
}

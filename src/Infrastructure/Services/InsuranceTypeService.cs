using AutoMapper;
using Core.DTOs;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class InsuranceTypeService : IInsuranceTypeService
{
    private readonly IInsuranceTypeRepository _repository;
    private readonly IMapper _mapper;

    public InsuranceTypeService(IInsuranceTypeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<InsuranceTypeDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllWithFirmAsync();
        return entities.Select(e => _mapper.Map<InsuranceTypeDto>(e)).ToList();
    }

    public async Task<InsuranceTypeDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<InsuranceTypeDto>(entity);
    }

    public async Task<InsuranceTypeDto> CreateAsync(CreateUpdateInsuranceTypeDto dto)
    {
        var entity = _mapper.Map<InsuranceType>(dto);
        var result = await _repository.AddAsync(entity);
        return _mapper.Map<InsuranceTypeDto>(result);
    }

    public async Task UpdateAsync(Guid id, CreateUpdateInsuranceTypeDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) throw new KeyNotFoundException();

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<List<InsuranceTypeDto>> GetByUserIdAsync(Guid userId)
    {
        var entities = await _repository.GetByUserIdAsync(userId);
        return entities.Select(e => _mapper.Map<InsuranceTypeDto>(e)).ToList();
    }

    public async Task<int> CountAsync()
    {
        return await _repository.GetActiveCountAsync();
    }
}

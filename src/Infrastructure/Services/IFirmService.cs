using Core.DTOs;

namespace Infrastructure.Services;

public interface IFirmService
{
    Task<List<FirmDto>> GetAllAsync();
    Task CreateAsync(CreateFirmDto dto);
    Task UpdateAsync(UpdateFirmDto dto);
}

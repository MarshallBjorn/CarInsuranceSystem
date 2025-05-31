using Core.DTOs;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FirmController : ControllerBase
{
    private readonly IFirmService _firmService;

    public FirmController(IFirmService firmService)
    {
        _firmService = firmService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var firms = await _firmService.GetAllAsync();
        return Ok(firms);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFirmDto dto)
    {
        await _firmService.CreateAsync(dto);
        return Ok("Firm created successfully.");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateFirmDto dto)
    {
        try
        {
            await _firmService.UpdateAsync(dto);
            return Ok("Firm updated successfully.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var firms = await _firmService.GetAllByUserAsync(userId);
        return Ok(firms);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount()
    {
        var count = await _firmService.CountAsync();

        if (count > 0) return Ok(count);

        return NotFound("No firms registered.");
    }
}

using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Core.DTOs;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class InsuranceTypesController : ControllerBase
{
    private readonly IInsuranceTypeService _service;

    public InsuranceTypesController(IInsuranceTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<InsuranceTypeDto>>> Get()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InsuranceTypeDto>> Get(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<InsuranceTypeDto>> Post(CreateUpdateInsuranceTypeDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, CreateUpdateInsuranceTypeDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<InsuranceTypeDto>>> GetByUserId(Guid userId)
    {
        var result = await _service.GetByUserIdAsync(userId);
        return Ok(result);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount()
    {
        var count = await _service.CountAsync();

        if (count > 0) return Ok(count);

        return NotFound("No current active insurances");
    }
}

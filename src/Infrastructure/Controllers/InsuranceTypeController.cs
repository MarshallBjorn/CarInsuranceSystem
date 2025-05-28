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
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, CreateUpdateInsuranceTypeDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}

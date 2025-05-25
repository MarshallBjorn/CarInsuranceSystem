using AutoMapper;
using Core.DTOs;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class InsuranceTypeController : ControllerBase
{
    private readonly InsuranceTypeService _service;
    private readonly IMapper _mapper;

    public InsuranceTypeController(InsuranceTypeService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InsuranceType>>> GetAll()
    {
        try
        {
            var types = await _service.GetAllInsuranceTypesAsync();

            var typesDto = _mapper.Map<List<InsuranceTypeDto>>(types);
            return Ok(typesDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
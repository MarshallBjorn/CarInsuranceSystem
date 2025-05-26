using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Core.DTOs;
using Core.Entities;
using FluentValidation;
using Infrastructure.Mapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Controllers;

/// <summary>
/// Controller for managing car-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CarController : ControllerBase
{
    private readonly CarService _carService;
    private readonly UserService _userService;
    private readonly IMapper _mapper;

    public CarController(CarService carService, UserService userService, IMapper mapper)
    {
        _carService = carService;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetAllCars()
    {
        var cars = await _carService.GetCarsAsync();
        return Ok(cars);
    }

    [HttpGet("{vin}")]
    public async Task<ActionResult<Car>> GetCarByVIN(string vin)
    {
        try
        {
            var car = await _carService.GetByVINAsync(vin);
            return Ok(car);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddCar([FromBody] Car car, [FromQuery] Guid userId)
    {
        try
        {
            var user = await _userService.GetByIdAsync(userId);
            var result = await _carService.AddCarAsync(car, user);

            return result
                ? CreatedAtAction(nameof(GetCarByVIN), new { vin = car.VIN }, car)
                : BadRequest("Failed to add car.");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<CarDto>>> GetCarsByUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found in token.");
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var user = await _userService.GetByIdAsync(userId);
            var cars = await _carService.GetCarsUserAsync(user);
            var carsDto = _mapper.Map<List<CarDto>>(cars);
            return Ok(carsDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [Authorize]
    [HttpPut("{vin}")]
    public async Task<IActionResult> UpdateCar(string vin, [FromBody] Car updatedCar)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _carService.UpdateCarDataAsync(vin, updatedCar);
        if (!success)
            return NotFound($"Car with VIN '{vin}' not found.");

        return NoContent(); // Or Ok(), depending on your frontend needs
    }
}
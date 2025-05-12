using Core.Entities;
using FluentValidation;
using Infrastructure.Services;
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

    public CarController(CarService carService, UserService userService)
    {
        _carService = carService;
        _userService = userService;
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

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Car>>> GetCarsByUser(Guid userId)
    {
        try
        {
            var user = await _userService.GetByIdAsync(userId);
            var cars = await _carService.GetCarsUserAsync(user);
            return Ok(cars);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
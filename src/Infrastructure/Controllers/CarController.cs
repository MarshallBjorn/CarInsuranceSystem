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

    /// <summary>
    /// Initializes a new instance of the <see cref="CarController"/> class.
    /// </summary>
    /// <param name="carService">The car service for handling car operations.</param>
    public CarController(CarService carService)
    {
        _carService = carService;
        _userService = ServiceLocator.GetService<UserService>();
    }

    /// <summary>
    /// Gets all cars in the system.
    /// </summary>
    /// <returns>A list of all cars.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetAllCars()
    {
        var cars = await _carService.GetCarsAsync();
        return Ok(cars);
    }

    /// <summary>
    /// Gets a car by its VIN.
    /// </summary>
    /// <param name="vin">The Vehicle Identification Number (VIN) of the car.</param>
    /// <returns>The car with the specified VIN.</returns>
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

    /// <summary>
    /// Adds a new car for a specific user.
    /// </summary>
    /// <param name="car">The car to add.</param>
    /// <param name="userId">The ID of the user associated with the car.</param>
    /// <returns>The created car.</returns>
    [HttpPost]
    [Consumes("application/json")]
    public async Task<ActionResult> AddCar([FromBody] Car car, [FromQuery] Guid userId)
    {
        try
        {
            if (car == null)
            {
                return BadRequest("Request body is empty or invalid.");
            }
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

    /// <summary>
    /// Gets all cars associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of cars for the specified user.</returns>
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
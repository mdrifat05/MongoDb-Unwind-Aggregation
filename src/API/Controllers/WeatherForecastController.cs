using API.Helpers;
using Domain.Dto;
using Domain.Params;
using Domain.Services.Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherForecastService _weatherForecastService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        IWeatherForecastService weatherForecastService)
    {
        _logger = logger;
        _weatherForecastService = weatherForecastService;
    }

    [HttpGet("GetWeatherForecast")]
    public async Task<ActionResult<Pagination<WeatherForecast>>> Get([FromQuery] GenericParams weatherForecastParams)
    {
        var list = await _weatherForecastService.GetWeatherForecasts(weatherForecastParams);
        var weatherForecasts = list as WeatherForecast[] ?? list.ToArray();
        return Ok(new Pagination<WeatherForecast>(weatherForecastParams.PageIndex, weatherForecastParams.PageSize,
            weatherForecasts.Length, weatherForecasts.ToList()));
    }

    [HttpPost("CreateWeatherForecast")]
    public async Task<IActionResult> CreateWeatherForecast([FromBody] WeatherForecastCreateDto createDto)
    {
        await _weatherForecastService.Create(createDto);
        return Ok();
    }

    [HttpPost("UpdateSummary")]
    public async Task<ActionResult> Update(string id, string summary)
    {
        await _weatherForecastService.UpdateSummary(id, summary);
        return Ok();
    }
}
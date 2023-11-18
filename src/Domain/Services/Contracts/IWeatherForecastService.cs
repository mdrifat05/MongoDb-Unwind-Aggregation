using Domain.Dto;
using Domain.Params;
using Entities;

namespace Domain.Services.Contracts;

public interface IWeatherForecastService
{
    Task<bool> Create(WeatherForecastCreateDto dto);

    Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(GenericParams genericParams);

    Task<bool> UpdateSummary(string itemId, string summary);
}
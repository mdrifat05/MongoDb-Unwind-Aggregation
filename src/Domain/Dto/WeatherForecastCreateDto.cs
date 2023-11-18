namespace Domain.Dto;

public class WeatherForecastCreateDto
{
    public int TemperatureC { get; set; }

    public int TemperatureF { get; set; }

    public string? Summary { get; set; }
}
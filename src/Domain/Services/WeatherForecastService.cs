using Domain.Dto;
using Domain.Params;
using Domain.Services.Contracts;
using Entities;
using MongoDB.Driver;
using Repositories.Contracts;

namespace Domain.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IMongoDbDataContextProvider _mongoDbDataContextProvider;
    private readonly IRepository _repository;

    public WeatherForecastService(IRepository repository, IMongoDbDataContextProvider mongoDbDataContextProvider)
    {
        _repository = repository;
        _mongoDbDataContextProvider = mongoDbDataContextProvider;
    }

    public async Task<bool> Create(WeatherForecastCreateDto dto)
    {
        var weatherForecast = new WeatherForecast
        {
            Date = DateTime.UtcNow,
            TemperatureC = dto.TemperatureC,
            TemperatureF = dto.TemperatureF,
            Summary = dto.Summary,
            CreatedDate = DateTime.UtcNow,
            ItemId = Guid.NewGuid().ToString(),
            LastUpdatedDate = DateTime.UtcNow
        };

        await _repository.SaveAsync(weatherForecast);
        return true;
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(GenericParams genericParams)
    {
        var weatherForecastList = await Task.Run(() =>
            _repository.GetItems<WeatherForecast>()
                .OrderByDescending(wf => wf.CreatedDate)
                .Skip(genericParams.PageIndex * genericParams.PageSize)
                .Take(genericParams.PageSize)
        );

        return weatherForecastList;

        // return await GetWeatherForecastWithBuilders();
    }

    public async Task<bool> UpdateSummary(string itemId, string summary)
    {
        var wf = await _repository.GetItemAsync<WeatherForecast>(wf => wf.ItemId == itemId);
        wf.Summary = summary;
        await _repository.UpdateAsync(forecast => forecast.ItemId == itemId, wf);
        return true;
    }

    private async Task<IEnumerable<WeatherForecast>> GetWeatherForecastWithBuilders()
    {
        var db = _mongoDbDataContextProvider.GetTenantDataContext();
        var collection = db.GetCollection<WeatherForecast>($"{nameof(WeatherForecast)}s");

        var filterBuilder = Builders<WeatherForecast>.Filter;
        var filter = filterBuilder.Lt(wf => wf.TemperatureC, 14);

        var sortBuilder = Builders<WeatherForecast>.Sort;
        var sort = sortBuilder.Ascending(wf => wf.TemperatureC);

        return await collection.Find(filter).Sort(sort).ToListAsync();
    }
}
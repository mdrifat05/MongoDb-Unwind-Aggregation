using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Repositories.Contracts;

namespace Repositories;

public class MongoDbDataContextProvider : IMongoDbDataContextProvider
{
    private readonly string _database;
    private readonly string _databaseConnectionString;
    private readonly ILogger<MongoDbDataContextProvider> _logger;
    private IMongoDatabase _mongoDatabaseBase;

    public MongoDbDataContextProvider(IConfiguration configuration, ILogger<MongoDbDataContextProvider> logger)
    {
        _logger = logger;
        _databaseConnectionString = configuration.GetSection("DatabaseConnectionString").Value!;
        _database = configuration.GetSection("DatabaseName").Value!;

        SetMongoDbConnection();
    }

    public string GetDbConnectionString()
    {
        return _databaseConnectionString;
    }

    public IMongoDatabase GetTenantDataContext()
    {
        return _mongoDatabaseBase;
    }

    public IQueryable<T> GetQueryableCollection<T>()
    {
        try
        {
            return _mongoDatabaseBase.GetCollection<T>($"{typeof(T).Name}s").AsQueryable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"MongoDbDataContextProvider -> GetQueryableCollection Error {ex.Message}");
        }

        return null;
    }

    public IMongoDatabase GetDatabase(string connectionString, string databaseName)
    {
        try
        {
            return new MongoClient(connectionString).GetDatabase(databaseName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"MongoDbDataContextProvider -> GetDatabase Error {ex.Message}");
        }

        return null;
    }

    private void SetMongoDbConnection()
    {
        try
        {
            _mongoDatabaseBase = new MongoClient(_databaseConnectionString).GetDatabase(_database);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"MongoDbDataContextProvider -> Mongo Connection Problem {ex.Message}");
        }
    }
}
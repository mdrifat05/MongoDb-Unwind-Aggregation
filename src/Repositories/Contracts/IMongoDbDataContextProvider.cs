using MongoDB.Driver;

namespace Repositories.Contracts;

public interface IMongoDbDataContextProvider
{
    IMongoDatabase GetTenantDataContext();

    string GetDbConnectionString();

    IQueryable<T> GetQueryableCollection<T>();

    IMongoDatabase GetDatabase(string connectionString, string databaseName);
}
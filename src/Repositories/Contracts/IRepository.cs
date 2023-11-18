using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Repositories.Contracts;

public interface IRepository
{
    IMongoCollection<BsonDocument> GetCollection(string entityName);

    Task<List<T>> GetItemsAsync<T>();
    IQueryable<T> GetItems<T>();
    IQueryable<T> GetItems<T>(Expression<Func<T, bool>> dataFilters);
    T? GetItem<T>(Expression<Func<T?, bool>> dataFilters);

    void Save<T>(List<T> dataList);
    void Save<T>(T data, string collectionName = "");

    Task SaveAsync<T>(string tenantId, T data);
    Task SaveAsync<T>(T data, string collectionName = "");
    Task SaveAsync<T>(string tenantId, List<T> dataList);
    Task SaveAsync<T>(List<T> dataList);

    Task<T> GetItemAsync<T>(Expression<Func<T, bool>> dataFilters);
    Task<T> GetItemAsync<T>(string tenantId, Expression<Func<T, bool>> dataFilters);

    void Update<T>(Expression<Func<T, bool>> dataFilters, T data);
    void UpdateMany<T>(Expression<Func<T, bool>> dataFilters, object data, string collectionName = "");
    Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, T data);
    Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, string tenantId, T data);
    Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, IDictionary<string, object> updates);
    Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, string tenantId, IDictionary<string, object> updates);
    Task UpdateManyAsync<T>(Expression<Func<T, bool>> dataFilters, IDictionary<string, object> updates);

    Task UpdateManyAsync<T>(Expression<Func<T, bool>> dataFilters, string tenantId,
        IDictionary<string, object> updates);

    void Delete<T>(Expression<Func<T, bool>> dataFilters);
    void Delete<T>(Expression<Func<T, bool>> dataFilters, string collectionName);
    Task DeleteAsync<T>(Expression<Func<T, bool>> dataFilters);
    Task DeleteAsync<T>(Expression<Func<T, bool>> dataFilters, string collectionName);

    Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> dataFilters);
    Task<BulkWriteError[]> SaveExpectingFailuresAsync<T>(List<T> dataList);
}
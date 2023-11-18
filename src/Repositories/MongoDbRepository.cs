using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Repositories.Contracts;

namespace Repositories;

public class MongoDbRepository : IRepository
{
    private readonly IMongoDbDataContextProvider _mongoDbDataContextProvider;

    public MongoDbRepository(
        IMongoDbDataContextProvider mongoDbDataContextProvider)
    {
        _mongoDbDataContextProvider = mongoDbDataContextProvider;
    }

    public IMongoCollection<BsonDocument> GetCollection(string entityName)
    {
        return _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<BsonDocument>($"{entityName}s");
    }

    public Task<List<T>> GetItemsAsync<T>()
    {
        var filter = Builders<T>.Filter.Empty;
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");
        return collection.Find(filter).ToListAsync();
    }

    public void Delete<T>(Expression<Func<T, bool>> dataFilters)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        collection.DeleteMany(dataFilters);
    }

    public void Delete<T>(Expression<Func<T, bool>> dataFilters, string collectionName)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>(collectionName);

        collection.DeleteMany(dataFilters);
    }

    public Task DeleteAsync<T>(Expression<Func<T, bool>> dataFilters)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        return collection.DeleteManyAsync(dataFilters);
    }

    public Task DeleteAsync<T>(Expression<Func<T, bool>> dataFilters, string collectionName)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>(collectionName);

        return collection.DeleteManyAsync(dataFilters);
    }

    public T? GetItem<T>(Expression<Func<T?, bool>> dataFilters)
    {
        return _mongoDbDataContextProvider.GetQueryableCollection<T>().FirstOrDefault(dataFilters!);
    }

    public Task<T> GetItemAsync<T>(Expression<Func<T, bool>> dataFilters)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        return collection.Find(dataFilters).FirstOrDefaultAsync();
    }

    public Task<T> GetItemAsync<T>(string tenantId, Expression<Func<T, bool>> dataFilters)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        return collection.Find(dataFilters).FirstOrDefaultAsync();
    }

    public IQueryable<T> GetItems<T>(Expression<Func<T, bool>> dataFilters)
    {
        return _mongoDbDataContextProvider.GetQueryableCollection<T>().Where(dataFilters);
    }

    public IQueryable<T> GetItems<T>()
    {
        return _mongoDbDataContextProvider.GetQueryableCollection<T>();
    }

    public void Save<T>(T data, string collectionName = "")
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext()
            .GetCollection<T>(string.IsNullOrWhiteSpace(collectionName) ? $"{typeof(T).Name}s" : collectionName);
        collection.InsertOne(data);
    }

    public void Save<T>(List<T> dataList)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");
        collection.InsertMany(dataList);
    }

    public Task SaveAsync<T>(string tenantId, T data)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");
        return collection.InsertOneAsync(data);
    }

    public Task SaveAsync<T>(T data, string collectionName = "")
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext()
            .GetCollection<T>(string.IsNullOrWhiteSpace(collectionName) ? $"{typeof(T).Name}s" : collectionName);
        return collection.InsertOneAsync(data);
    }

    public Task SaveAsync<T>(string tenantId, List<T> dataList)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");
        return collection.InsertManyAsync(dataList);
    }

    public Task SaveAsync<T>(List<T> dataList)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");
        return collection.InsertManyAsync(dataList);
    }

    public void Update<T>(Expression<Func<T, bool>> dataFilters, T data)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        collection.ReplaceOne(dataFilters, data);
    }

    public Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, T data)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        return collection.ReplaceOneAsync(dataFilters, data);
    }

    public void UpdateMany<T>(Expression<Func<T, bool>> dataFilters, object data, string collectionName = "")
    {
        var fieldValuePairs = GetValues(data);
        UpdateDefinition<T>? update = null;


        foreach (var fieldValuePair in fieldValuePairs)
            if (update == null)
            {
                if (fieldValuePair.Value is string[])
                    update = Builders<T>.Update.Set(fieldValuePair.Key, (string[])fieldValuePair.Value);
                else
                    update = Builders<T>.Update.Set(fieldValuePair.Key, fieldValuePair.Value);
            }
            else
            {
                if (fieldValuePair.Value is string[])
                    update = update.Set(fieldValuePair.Key, (string[])fieldValuePair.Value);
                else
                    update = update.Set(fieldValuePair.Key, fieldValuePair.Value);
            }


        if (string.IsNullOrEmpty(collectionName))
            collectionName = $"{typeof(T).Name}s";

        _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>(collectionName)
            .UpdateMany(dataFilters, update);
    }

    public Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, IDictionary<string, object> updates)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        var updateDocument = new BsonDocument(true);

        foreach (var update in updates)
        {
            var change = new BsonDocument(update.Key, BsonValue.Create(update.Value));
            updateDocument.Add("$set", change);
        }

        return collection.UpdateOneAsync(dataFilters, updateDocument);
    }

    public Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, string tenantId,
        IDictionary<string, object> updates)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        var updateDocument = new BsonDocument(true);

        foreach (var update in updates)
        {
            var change = new BsonDocument(update.Key, BsonValue.Create(update.Value));
            updateDocument.Add("$set", change);
        }

        return collection.UpdateOneAsync(dataFilters, updateDocument);
    }

    public Task UpdateManyAsync<T>(Expression<Func<T, bool>> dataFilters, IDictionary<string, object> updates)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        var updateDocument = new BsonDocument(true);

        foreach (var update in updates)
        {
            var change = new BsonDocument(update.Key, BsonValue.Create(update.Value));
            updateDocument.Add("$set", change);
        }

        return collection.UpdateManyAsync(dataFilters, updateDocument);
    }

    public Task UpdateManyAsync<T>(Expression<Func<T, bool>> dataFilters, string tenantId,
        IDictionary<string, object> updates)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        var updateDocument = new BsonDocument(true);

        foreach (var update in updates)
        {
            var change = new BsonDocument(update.Key, BsonValue.Create(update.Value));
            updateDocument.Add("$set", change);
        }

        return collection.UpdateManyAsync(dataFilters, updateDocument);
    }

    public Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, string tenantId, T data)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        return collection.ReplaceOneAsync(dataFilters, data);
    }

    public Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> dataFilters)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        return collection.Find(dataFilters).AnyAsync();
    }

    public async Task<BulkWriteError[]> SaveExpectingFailuresAsync<T>(List<T> dataList)
    {
        var collection = _mongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>($"{typeof(T).Name}s");

        try
        {
            await collection.InsertManyAsync(dataList, new InsertManyOptions { IsOrdered = false });
        }
        catch (MongoBulkWriteException e)
        {
            return e.WriteErrors.Select(error => new BulkWriteError { Message = error.Message, Index = error.Index })
                .ToArray();
        }

        return Array.Empty<BulkWriteError>();
    }

    private static Dictionary<string, object?> GetValues(object obj)
    {
        return obj
            .GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(obj) ?? null);
    }
}
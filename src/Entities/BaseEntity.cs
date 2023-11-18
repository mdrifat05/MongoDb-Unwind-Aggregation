using MongoDB.Bson.Serialization.Attributes;

namespace Entities;

public class BaseEntity
{
    [BsonId] public string? ItemId { get; set; }

    public virtual string? CreatedBy { get; set; }
    public virtual DateTime CreatedDate { get; set; }

    public virtual string? LastUpdatedBy { get; set; }
    public virtual DateTime LastUpdatedDate { get; set; }
}
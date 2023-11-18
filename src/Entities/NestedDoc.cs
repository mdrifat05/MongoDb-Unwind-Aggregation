using MongoDB.Bson.Serialization.Attributes;

namespace Entities;

public class NestedDoc : BaseEntity
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF { get; set; }

    public string? Summary { get; set; }

    public List<FirstArray> FirstArray { get; set; }
}

public class FirstArray
{
    [BsonElement("a")] public string A { get; set; }

    [BsonElement("b")] public string B { get; set; }

    public List<SecondArray> SecondArray { get; set; }
}

public class SecondArray
{
    [BsonElement("a")] public string A { get; set; }

    [BsonElement("b")] public string B { get; set; }

    public List<ThirdArray> ThirdArray { get; set; }
}

public class ThirdArray
{
    [BsonElement("a")] public string A { get; set; }

    [BsonElement("b")] public string B { get; set; }

    public List<ForthArray> ForthArray { get; set; }
}

public class ForthArray
{
    [BsonElement("a")] public string A { get; set; }

    [BsonElement("b")] public string B { get; set; }
}
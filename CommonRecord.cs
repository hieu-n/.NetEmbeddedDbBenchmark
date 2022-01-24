using MongoDB.Bson;

namespace DbBench;

public class CommonRecord
{
    public string MongoDbObjectId { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public long Timestamp { get; set; }
}
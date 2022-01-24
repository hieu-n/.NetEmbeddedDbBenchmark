using System.Text.Json;
using DBreeze;
using MongoDB.Bson;

namespace DbBench;

public class MyDBreeze : BenchDb
{
    
    public override Task BenchWrite()
    {
        using var engine = new DBreezeEngine(GetDatabaseStoragePath());
        engine.Scheme.DeleteTable("CommonRecord");
        using var tran = engine.GetTransaction();
        foreach (var numb in Enumerable.Range(0, NumberOfDocs))
        {
            tran.Insert("CommonRecord", ObjectId.GenerateNewId().ToString(), JsonSerializer.Serialize( new CommonRecord()
            {
                Name = "John", Age = numb, Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }));
            tran.Commit();
        }
        return Task.CompletedTask;
    }

    public override Task<int> BenchRead()
    {
         using var engine = new DBreezeEngine(GetDatabaseStoragePath());
         using var tran = engine.GetTransaction();
         var total = 0;
         foreach (var row in tran.SelectForward<string, string>("CommonRecord"))
         {
             var o = JsonSerializer.Deserialize<CommonRecord>(row.Value)!;
             total += o.Age;
         }
         return Task.FromResult(total);
    }
}
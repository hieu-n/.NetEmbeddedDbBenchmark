using System.Diagnostics;
using LiteDB;

namespace DbBench;

public class MyLiteDb : BenchDb
{

    public override Task BenchWrite()
    {
        using var db = new LiteDatabase(GetDatabaseStoragePath());
        var col = db.GetCollection<CommonRecord>(nameof(CommonRecord));
        col.DeleteAll();

        foreach (var numb in Enumerable.Range(0, NumberOfDocs))
        {
            col.Insert(new CommonRecord()
            {
                Name = "John",
                Age = numb,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
        }

        return Task.CompletedTask;
    }

    public override Task<int> BenchRead()
    {
        using var db = new LiteDatabase(GetDatabaseStoragePath());
        var col = db.GetCollection<CommonRecord>(nameof(CommonRecord));
        var total = 0;
        foreach (var realmBench in col.Query().ToEnumerable())
        {
            total += realmBench.Age;
        }

        Debug.WriteLine(total);
        return Task.FromResult(total);
    }

}
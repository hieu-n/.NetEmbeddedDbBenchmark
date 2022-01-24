# DotNetEmbeddedDbBenchmark
Micro benchmarking some embedded database for C#/.Net:

- Realm DB: https://docs.mongodb.com/realm/sdk/dotnet/
- LiteDB: https://www.litedb.org/
- DBreeze: https://github.com/hhblaze/DBreeze
- RavenDB (embdded mode): https://ravendb.net/docs/article-page/5.3/csharp/server/embedded
- Sqlite: https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=netcore-cli
- Sqlite + Dapper: https://github.com/DapperLib/Dapper

The code is very simple: Insert `n` records, measure the write time. Read `n` records, measure the read time. `n` is configurable via `BenchDb.NumberOfDocs`. For an example, the code for benchmarking LiteDB:

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

# Prerequisite:

- Dot Net Core SDK 6: https://dotnet.microsoft.com/en-us/download- 

# How to run this bench:

    $ git clone https://github.com/hieu-n/DotNetEmbeddedDbBenchmark.git
    $ cd DotNetEmbeddedDbBenchmark
    $ dotnet run

# Sample output:


    Attempt 0 -------------------------------------------------
    MyDBreeze:
    Write: 00:02:36.7131439
    Read:  00:00:00.0321113
    Size On Disk: 181.000 kB
    MyPlainSqlite:
    Write: 00:00:17.6243079
    Read:  00:00:00.0053830
    Size On Disk: 94.000 kB
    MyDapperSqlite:
    Write: 00:00:16.7423329
    Read:  00:00:00.0421642
    Size On Disk: 94.000 kB
    MyLiteDb:
    Write: 00:00:00.5120954
    Read:  00:00:00.0209019
    Size On Disk: 253.000 kB
    MyRealmDb:
    Write: 00:00:04.4439233
    Read:  00:00:00.0092069
    Size On Disk: 131.000 kB
    MyRavenDb:
    Write: 00:00:04.2224211
    Read:  00:00:00.0693350
    Size On Disk: 80,806.000 kB

# Contribution and Feedback are weblcome.



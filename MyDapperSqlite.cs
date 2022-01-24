using Dapper;
using Microsoft.Data.Sqlite;

namespace DbBench;

public class MyDapperSqlite : BenchDb
{

    public override async Task BenchWrite()
    {
        // if (File.Exists(Fp)) File.Delete(Fp);
        using var conn = new SqliteConnection(new SqliteConnectionStringBuilder()
        {
            DataSource = GetDatabaseStoragePath()
        }.ToString());
        await conn.ExecuteAsync(@"
create table IF NOT EXISTS CommonRecord
(
    MongoDbObjectId text
        constraint Bench_pk
            primary key,
    Name            text    default '' not null,
    Age             integer default 0 not null,
    Timestamp       integer default 0 not null
);
").ConfigureAwait(false);

        await conn.ExecuteAsync("DELETE FROM CommonRecord;").ConfigureAwait(false);
        const string insertQuery =
            "insert into CommonRecord (MongoDbObjectId, Name, Age, Timestamp) " +
            "Values (@MongoDbObjectId, @Name, @Age, @Timestamp)";

        foreach (var numb in Enumerable.Range(0, NumberOfDocs))
        {
            await conn.ExecuteAsync(insertQuery, new CommonRecord() {Name = "John", Age = numb, Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()})
                .ConfigureAwait(false);
        }
    }

    public override async Task<int> BenchRead()
    {
        using var conn = new SqliteConnection(new SqliteConnectionStringBuilder()
        {
            DataSource = GetDatabaseStoragePath()
        }.ToString());
        var query = await conn.QueryAsync<CommonRecord>("select * from CommonRecord").ConfigureAwait(false);
        var total = 0;
        foreach (var myDapperSqliteBench in query)
        {
            total += myDapperSqliteBench.Age;
        }
        return total;
    }

}
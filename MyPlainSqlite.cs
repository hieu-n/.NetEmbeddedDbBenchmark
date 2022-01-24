using Microsoft.Data.Sqlite;
using MongoDB.Bson;

namespace DbBench;

public class MyPlainSqlite : BenchDb
{

    public override async Task BenchWrite()
    {
        using var conn = new SqliteConnection(new SqliteConnectionStringBuilder()
        {
            DataSource = GetDatabaseStoragePath()
        }.ToString());
        await conn.OpenAsync().ConfigureAwait(false);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
 create table IF NOT EXISTS CommonRecord
 (
     MongoDbObjectId text
         constraint Bench_pk
             primary key,
     Name            text    default '' not null,
     Age             integer default 0 not null,
     Timestamp       integer default 0 not null
 );
";
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        cmd.CommandText = "DELETE FROM CommonRecord;";
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);


        foreach (var numb in Enumerable.Range(0, NumberOfDocs))
        {
            using var cmd2 = conn.CreateCommand();
            cmd2.CommandText =
                "insert into CommonRecord (MongoDbObjectId, Name, Age, Timestamp) " +
                "Values (@MongoDbObjectId, @Name, @Age, @Timestamp);";
            cmd2.Parameters.AddWithValue("@MongoDbObjectId", ObjectId.GenerateNewId().ToString());
            cmd2.Parameters.AddWithValue("@Name", "John");
            cmd2.Parameters.AddWithValue("@Age", numb);
            cmd2.Parameters.AddWithValue("@Timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            // cmd.Prepare();
            // cmd.ExecuteNonQuery();
            await cmd2.PrepareAsync().ConfigureAwait(false);
            await cmd2.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
    }

    public override async Task<int> BenchRead()
    {
        using var conn = new SqliteConnection(new SqliteConnectionStringBuilder()
        {
            DataSource = GetDatabaseStoragePath()
        }.ToString());
        await conn.OpenAsync().ConfigureAwait(false);
        using var cmd = new SqliteCommand("SELECT * FROM CommonRecord", conn);
        using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        var total = 0;
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            total += reader.GetInt32(2);
        }

        return total;
    }
}
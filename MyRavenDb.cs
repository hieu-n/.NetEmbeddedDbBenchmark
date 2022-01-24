using Raven.Embedded;

namespace DbBench;

public class MyRavenDb : BenchDb
{

    public static bool IsServerStarted = false;

    public override async Task BenchWrite()
    {
        if (!IsServerStarted)
        {
            EmbeddedServer.Instance.StartServer(new ServerOptions() {DataDirectory = GetDatabaseStoragePath(), FrameworkVersion = "6.0.0"});
            IsServerStarted = true;
        }

        using var store = await EmbeddedServer.Instance.GetDocumentStoreAsync("Embedded").ConfigureAwait(false);
        using var session = store.OpenSession();
        foreach (var commonRecord in session.Query<CommonRecord>())
        {
            session.Delete(commonRecord);
        }
        session.SaveChanges();
        
        foreach (var numb in Enumerable.Range(0, NumberOfDocs))
        {
            session.Store(new CommonRecord()
            {
                Name = "John",
                Age = numb,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
        }
        session.SaveChanges();
    }

    public override async Task<int> BenchRead()
    {
        using var store = await EmbeddedServer.Instance.GetDocumentStoreAsync("Embedded").ConfigureAwait(false);
        using var session = store.OpenSession();
        var total = 0;
        foreach (var commonRecord in session.Query<CommonRecord>())
        {
            total += commonRecord.Age;
        }

        return total;
    }
}
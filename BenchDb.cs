namespace DbBench;

public abstract class BenchDb
{
    public const int NumberOfDocs = 1000;

    public string GetDatabaseStoragePath()
    {
        const string dp = $"/tmp/{nameof(BenchDb)}";
        if (!Directory.Exists(dp))
            Directory.CreateDirectory(dp);
        return $"{dp}/{GetType().Name}";
    }

    public abstract Task BenchWrite();
    public abstract Task<int> BenchRead();

    public async Task BenchMaster()
    {
        Console.WriteLine($"{GetType().Name}:");
        var time = DateTime.UtcNow;
        await BenchWrite().ConfigureAwait(false);
        Console.WriteLine($"Write: {DateTime.UtcNow - time}");
        time = DateTime.UtcNow;
        var total = await BenchRead().ConfigureAwait(false);

        if (total != Enumerable.Range(0, NumberOfDocs).Sum()) throw new Exception($"Wrong total: {total}");

        Console.WriteLine($"Read:  {DateTime.UtcNow - time}");
        long dbStorageSize = 0;
        var path = GetDatabaseStoragePath();
        if (Directory.Exists(path))
        {
            dbStorageSize = GetDirectorySize(path);
        }
        else
        {
            dbStorageSize = (new FileInfo(path)).Length;
        }

        Console.WriteLine($"Size On Disk: {dbStorageSize/1000:n} kB");
    }

    private static long GetDirectorySize(string folderPath)
    {
        var di = new DirectoryInfo(folderPath);
        return di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);
    }
}
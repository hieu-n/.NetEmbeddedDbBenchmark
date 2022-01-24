// See https://aka.ms/new-console-template for more information


using DbBench;

foreach (var i in Enumerable.Range(0, 3))
{
    Console.WriteLine($"Attempt {i} -------------------------------------------------");
    foreach (var benchDb in new BenchDb[]{new MyDBreeze(), new MyPlainSqlite(), new MyDapperSqlite(), new MyLiteDb(), new MyRealmDb(), new MyRavenDb(), })
    {
        await benchDb.BenchMaster().ConfigureAwait(false);
    }
}
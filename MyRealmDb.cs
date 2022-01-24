using MongoDB.Bson;
using Realms;

namespace DbBench;


public class MyRealmDbBench : RealmObject
{
    [PrimaryKey]
    [MapTo("_id")]
    public string MongoDbObjectId { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Name { get; set; } = "";
    public int Age { get; set; }
    public long Timestamp { get; set; }
}


public class MyRealmDb : BenchDb
{
     public async Task<Realm> MkRealm(bool deleteIfExist = false)
     {
         var config = new RealmConfiguration(GetDatabaseStoragePath());
         if (deleteIfExist) Realm.DeleteRealm(config);
         return await Realm.GetInstanceAsync(config).ConfigureAwait(false);
     }
   
     public override async Task BenchWrite()
     {
         using var realm = await MkRealm().ConfigureAwait(false);
         await realm.WriteAsync(tempRealm => { tempRealm.RemoveAll<MyRealmDbBench>(); }).ConfigureAwait(false);
         foreach (var numb in Enumerable.Range(0, NumberOfDocs))
         {
             await realm.WriteAsync((tempRealm) =>
             {
                 tempRealm.Add(new MyRealmDbBench()
                 {
                     Name = "John",
                     Age = numb,
                     Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                 });
             }).ConfigureAwait(false);
         }
     }
 
     public override async Task<int> BenchRead()
     {
         using var realm = await MkRealm().ConfigureAwait(false);
         var total = 0;
         foreach (var realmBench in realm.All<MyRealmDbBench>())
         {
             total += realmBench.Age;
         }

         return total;
     }
}
using KitanoUserService.API.Models.ExecuteModels;
using MongoDB.Driver;

namespace KitanoUserService.API
{
    public class AppDbContext
    {
        private readonly IMongoDatabase mongoDatabase;

        //public AppDbContext(IMongoClient client, string dbName)
        //{

        //    mongoDatabase = client.GetDatabase(dbName);
        //}

        public IMongoCollection<SystemLogModel> SystemLogs => mongoDatabase.GetCollection<SystemLogModel>("SYSTEM_LOG");
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using BastetAPI.Entities;
using MongoDB.Driver;

namespace BastetFTMAPI.Repositories
{
    public class MongoDbPhonesRepository : IPhonesRepository
    {
        private const string DB_NAME = "BastetFTM";
        private const string Collection_Name = "Client";
        private readonly IMongoCollection<Client> clientCollection;
        private readonly FilterDefinitionBuilder<Client> filterBuilder = Builders<Client>.Filter;

        public MongoDbPhonesRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(DB_NAME);
            clientCollection = database.GetCollection<Client>(Collection_Name);
        }

        public async Task CreatePhoneAsync(Guid cId, PhoneInfo p)
        {
            var clientFilter = filterBuilder.Eq(c => c.Id, cId);
            var mobileFilter = Builders<Client>.Update.Push(x => x.PhoneNumbers, p);
            await clientCollection.UpdateOneAsync(clientFilter, mobileFilter);
        }
        public async Task DeletePhoneAsync(Guid cId, Guid mId)
        {
            var filter = filterBuilder.Where(x => x.Id == cId);
            var update = Builders<Client>.Update.PullFilter(x => x.PhoneNumbers,
                                                            Builders<PhoneInfo>.Filter.Where(x => x.Id == mId));
            await clientCollection.UpdateOneAsync(filter, update);
        }
        public async Task<PhoneInfo> GetPhoneAsync(Guid cId, Guid mId)
        {
            var filter = filterBuilder.Eq(c => c.Id, cId);

            var mobile = (await clientCollection.Find(filter).ToListAsync())
                .Select(x => x.PhoneNumbers)
                .ToList()[0]
                .SingleOrDefault(x => x.Id == mId);

            return mobile;
        }
    }
}

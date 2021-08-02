using System;
using System.Linq;
using System.Threading.Tasks;
using BastetAPI.Entities;
using MongoDB.Driver;

namespace BastetFTMAPI.Repositories
{
    public class MongoDbAddressesRepository : IAddressesRepository
    {
        private const string DB_NAME = "BastetFTM";
        private const string Collection_Name = "Client";
        private readonly IMongoCollection<Client> clientCollection;
        private readonly FilterDefinitionBuilder<Client> filterBuilder = Builders<Client>.Filter;

        public MongoDbAddressesRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(DB_NAME);
            clientCollection = database.GetCollection<Client>(Collection_Name);
        }

        public async Task CreateAddressAsync(Guid cId, AddressInfo a)
        {
            var clientFilter = filterBuilder.Eq(c => c.Id, cId);
            var addressFilter = Builders<Client>.Update.Push(x => x.Addresses, a);
            await clientCollection.UpdateOneAsync(clientFilter, addressFilter);
        }
        public async Task DeleteAddressAsync(Guid cId, Guid aId)
        {
            var filter = filterBuilder.Where(x => x.Id == cId);
            var update = Builders<Client>.Update.PullFilter(x => x.Addresses,
                                                            Builders<AddressInfo>.Filter.Where(x => x.Id == aId));
            await clientCollection.UpdateOneAsync(filter, update);
        }
        public async Task<AddressInfo> GetAddressAsync(Guid cId, Guid aId)
        {
            var address = (await clientCollection.Aggregate()
                 .Match(filterBuilder.Eq(x => x.Id, cId) & filterBuilder.ElemMatch(x => x.Addresses, Builders<AddressInfo>.Filter.Eq(x => x.Id, aId)))
                 .ToListAsync())
                 .Select(x => x.Addresses)
                 .FirstOrDefault()
                 .SingleOrDefault();
            return address;
        }

    }
}

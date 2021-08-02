using System;
using System.Linq;
using System.Threading.Tasks;
using BastetAPI.Entities;
using MongoDB.Driver;

namespace BastetFTMAPI.Repositories
{
    public class MongoDbEmailRepository : IEmailRepository
    {
        private const string DB_NAME = "BastetFTM";
        private const string Collection_Name = "Client";
        private readonly IMongoCollection<Client> clientCollection;
        private readonly FilterDefinitionBuilder<Client> filterBuilder = Builders<Client>.Filter;

        public MongoDbEmailRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(DB_NAME);
            clientCollection = database.GetCollection<Client>(Collection_Name);
        }
        public async Task CreateEmailAsync(Guid cId, EmailInfo e)
        {
            var clientFilter = filterBuilder.Eq(c => c.Id, cId);
            var emailFilter = Builders<Client>.Update.Push(x => x.Emails, e);
            await clientCollection.UpdateOneAsync(clientFilter, emailFilter);
        }
        public async Task DeleteEmailAsync(Guid cId, Guid eId)
        {
            var filter = filterBuilder.Where(x => x.Id == cId);
            var update = Builders<Client>.Update.PullFilter(x => x.Emails,
                                                            Builders<EmailInfo>.Filter.Where(x => x.Id == eId));
            await clientCollection.UpdateOneAsync(filter, update);
        }
        public async Task<EmailInfo> GetEmailAsync(Guid cId, Guid eId)
        {
            var email = (await clientCollection.Aggregate()
                .Match(filterBuilder.Eq(x => x.Id, cId) & filterBuilder.ElemMatch(x => x.Emails, Builders<EmailInfo>.Filter.Eq(x => x.Id, eId)))
                .ToListAsync())
                .Select(x => x.Emails)
                .FirstOrDefault()
                .SingleOrDefault();
            return email;
        }
    }
}

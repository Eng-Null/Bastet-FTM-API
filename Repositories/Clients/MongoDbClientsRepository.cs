using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BastetAPI.Entities;
using BastetFTMAPI;
using BastetFTMAPI.Parameters;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BastetAPI.Repositories
{
    public class MongoDbClientsRepository : IClientsRepository
    {
        private const string DB_NAME = "BastetFTM";
        private const string Collection_Name = "Client";
        private readonly IMongoCollection<Client> clientCollection;
        private readonly FilterDefinitionBuilder<Client> filterBuilder = Builders<Client>.Filter;

        public MongoDbClientsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(DB_NAME);
            clientCollection = database.GetCollection<Client>(Collection_Name);
        }

        #region Create
        public async Task CreateClientAsync(Client c)
        {
            await clientCollection.InsertOneAsync(c);
        }
        public async Task CreateClientsAsync(List<Client> c)
        {
            await clientCollection.InsertManyAsync(c);
        }
        #endregion Create

        #region Delete
        public async Task DeleteClientAsync(Guid id)
        {
            var filter = filterBuilder.Eq(c => c.Id, id);
            await clientCollection.DeleteOneAsync(filter);
        }
        #endregion Delete

        #region Get
        public async Task<Client> GetClientAsync(Guid id)
        {
            var filter = filterBuilder.Eq(c => c.Id, id);
            return await clientCollection.Find(filter).SingleOrDefaultAsync();
        }
        public async Task<PagedList<Client>> GetClientsAsync(PaginationParameters clientParameters, string name = null)
        {
            var clients = (await clientCollection.Find(new BsonDocument())
                                         .ToListAsync()).AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                clients = clients.Where(c =>
                                        c.Firstname.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                                        c.Lastname.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            return PagedList<Client>.ToPagedList(clients, clientParameters.PageNumber, clientParameters.PageSize);
        }
        #endregion Get

        #region Update
        public async Task UpdateClientAsync(Client c)
        {
            var filter = filterBuilder.Eq(ec => ec.Id, c.Id);
            await clientCollection.ReplaceOneAsync(filter, c);
        }
        #endregion Update
    }
}

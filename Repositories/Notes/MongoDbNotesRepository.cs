using System;
using System.Linq;
using System.Threading.Tasks;
using BastetAPI.Entities;
using MongoDB.Driver;

namespace BastetFTMAPI.Repositories
{
    public class MongoDbNotesRepository : INotesRepository
    {
        private const string DB_NAME = "BastetFTM";
        private const string Collection_Name = "Client";
        private readonly IMongoCollection<Client> clientCollection;
        private readonly FilterDefinitionBuilder<Client> filterBuilder = Builders<Client>.Filter;

        public MongoDbNotesRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(DB_NAME);
            clientCollection = database.GetCollection<Client>(Collection_Name);
        }
        public async Task CreateNoteAsync(Guid cId, NoteInfo n)
        {
            var clientFilter = filterBuilder.Eq(c => c.Id, cId);
            var noteFilter = Builders<Client>.Update.Push(x => x.Notes, n);
            await clientCollection.UpdateOneAsync(clientFilter, noteFilter);
        }
        public async Task DeleteNoteAsync(Guid cId, Guid nId)
        {
            var filter = filterBuilder.Where(x => x.Id == cId);
            var update = Builders<Client>.Update.PullFilter(x => x.Notes,
                                                            Builders<NoteInfo>.Filter.Where(x => x.Id == nId));
            await clientCollection.UpdateOneAsync(filter, update);
        }
        public async Task<NoteInfo> GetNoteAsync(Guid cId, Guid nId)
        {
            var note = (await clientCollection.Aggregate()
                .Match(filterBuilder.Eq(x => x.Id, cId) & filterBuilder.ElemMatch(x => x.Notes, Builders<NoteInfo>.Filter.Eq(x => x.Id, nId)))
                .ToListAsync())
                .Select(x => x.Notes)
                .FirstOrDefault()
                .SingleOrDefault();
            return note;
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using BastetAPI.Entities;
using BastetFTMAPI.Parameters;
using MongoDB.Driver;

namespace BastetFTMAPI.Repositories
{
    public class MongoDbTicketsRepository : ITicketsRepository
    {
        private const string DB_NAME = "BastetFTM";
        private const string Collection_Name = "Client";
        private readonly IMongoCollection<Client> clientCollection;
        private readonly FilterDefinitionBuilder<Client> filterBuilder = Builders<Client>.Filter;

        public MongoDbTicketsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(DB_NAME);
            clientCollection = database.GetCollection<Client>(Collection_Name);
        }

        public async Task CreateTicketAsync(Guid cId, TicketInfo e)
        {
            var clientFilter = filterBuilder.Eq(c => c.Id, cId);
            var ticketFilter = Builders<Client>.Update.Push(x => x.Tickets, e);
            await clientCollection.UpdateOneAsync(clientFilter, ticketFilter);
        }
        public async Task CreateTicketsNoteAsync(Guid cId, Guid tId, NoteInfo n)
        {
            var clientFilter = filterBuilder.Eq(c => c.Id, cId);
            var ticketFilter = Builders<Client>.Update.Push(x => x.Tickets[-1].Notes, n);
            await clientCollection.UpdateOneAsync(clientFilter, ticketFilter);
        }
        public async Task DeleteTicketAsync(Guid cId, Guid tId)
        {
            var filter = filterBuilder.Where(x => x.Id == cId);
            var update = Builders<Client>.Update.PullFilter(x => x.Tickets,
                                                            Builders<TicketInfo>.Filter.Where(x => x.Id == tId));
            await clientCollection.UpdateOneAsync(filter, update);
        }
        public Task DeleteTicketsNoteAsync(Guid cId, Guid tId, Guid nId)
        {
            throw new NotImplementedException();
        }
        public async Task<PagedList<TicketInfo>> GetTicketsAsync(PaginationParameters TicketParameters, Guid id)
        {
            var filter = filterBuilder.Eq(c => c.Id, id);
            var tickets = (await clientCollection.Find(filter)
                                                 .ToListAsync())
                                                 .FirstOrDefault().Tickets
                                                 .AsQueryable();

            return PagedList<TicketInfo>.ToPagedList(tickets, TicketParameters.PageNumber, TicketParameters.PageSize);
        }
        public async Task<TicketInfo> GetTicketAsync(Guid cId, Guid tId)
        {
            var ticket = (await clientCollection.Aggregate()
                 .Match(filterBuilder.Eq(x => x.Id, cId) & filterBuilder.ElemMatch(x => x.Tickets, Builders<TicketInfo>.Filter.Eq(x => x.Id, tId)))
                 .ToListAsync())
                 .Select(x => x.Tickets)
                 .FirstOrDefault()
                 .SingleOrDefault();
            return ticket;
        }
        public async Task<NoteInfo> GetTicketsNoteAsync(Guid cId, Guid tId, Guid nId)
        {
            var ticketNote = (await clientCollection.Aggregate()
                 .Match(filterBuilder.Eq(x => x.Id, cId) &
                        filterBuilder.ElemMatch(x => x.Tickets, Builders<TicketInfo>.Filter.Eq(x => x.Id, tId)) &
                        filterBuilder.ElemMatch(x => x.Tickets[-1].Notes, Builders<NoteInfo>.Filter.Eq(x => x.Id, nId)))
                 .ToListAsync())
                 .Select(x => x.Tickets[-1].Notes)
                 .FirstOrDefault()
                 .SingleOrDefault();
            return ticketNote;
        }
    }
}

using System;
using System.Threading.Tasks;
using BastetAPI.Entities;
using BastetFTMAPI.Parameters;

namespace BastetFTMAPI.Repositories
{
    public interface ITicketsRepository
    {
        Task<PagedList<TicketInfo>> GetTicketsAsync(PaginationParameters TicketParameters, Guid id);

        Task<TicketInfo> GetTicketAsync(Guid cId, Guid tId);

        Task<NoteInfo> GetTicketsNoteAsync(Guid cId, Guid tId, Guid nId);

        Task CreateTicketAsync(Guid cId, TicketInfo e);

        Task CreateTicketsNoteAsync(Guid cId, Guid tId, NoteInfo n);

        Task DeleteTicketAsync(Guid cId, Guid tId);

        Task DeleteTicketsNoteAsync(Guid cId, Guid tId, Guid nId);
    }
}

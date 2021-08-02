using System;
using System.Threading.Tasks;
using BastetAPI.Entities;

namespace BastetFTMAPI.Repositories
{
    public interface INotesRepository
    {
        Task<NoteInfo> GetNoteAsync(Guid cId, Guid nId);
        Task CreateNoteAsync(Guid cId, NoteInfo n);
        Task DeleteNoteAsync(Guid cId, Guid nId);
    }
}

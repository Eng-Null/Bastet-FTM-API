using System;
using System.Threading.Tasks;
using BastetAPI.Entities;

namespace BastetFTMAPI.Repositories
{
    public interface IEmailRepository
    {
        Task<EmailInfo> GetEmailAsync(Guid cId, Guid eId);
        Task CreateEmailAsync(Guid cId, EmailInfo e);
        Task DeleteEmailAsync(Guid cId, Guid eId);
    }
}

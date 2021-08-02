using System;
using System.Threading.Tasks;
using BastetAPI.Entities;

namespace BastetFTMAPI.Repositories
{
    public interface IPhonesRepository
    {
        Task<PhoneInfo> GetPhoneAsync(Guid cId, Guid pId);
        Task CreatePhoneAsync(Guid cId, PhoneInfo p);
        Task DeletePhoneAsync(Guid cId, Guid pId);
    }
}

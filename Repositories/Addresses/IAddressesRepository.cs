using System;
using System.Threading.Tasks;
using BastetAPI.Entities;

namespace BastetFTMAPI.Repositories
{
    public interface IAddressesRepository
    {
        Task<AddressInfo> GetAddressAsync(Guid cId, Guid aId);
        Task CreateAddressAsync(Guid cId, AddressInfo a);
        Task DeleteAddressAsync(Guid cId, Guid aId);
    }
}

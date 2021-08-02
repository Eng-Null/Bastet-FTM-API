using BastetAPI.Entities;
using BastetFTMAPI;
using BastetFTMAPI.Parameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BastetAPI.Repositories
{
    public interface IClientsRepository
    {
        Task<PagedList<Client>> GetClientsAsync(PaginationParameters clientParameters, string name = null);

        Task<Client> GetClientAsync(Guid id);

        Task CreateClientAsync(Client c);

        Task CreateClientsAsync(List<Client> c);

        Task UpdateClientAsync(Client c);

        Task DeleteClientAsync(Guid id);

    }
}

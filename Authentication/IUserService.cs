using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BastetFTMAPI.Authentication
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string userNameHash, string password);
        Task CreateUserAsync(User user);
        Task<User> GetUserAsync(Guid id);
        Task<List<User>> GetUsersAsync();
    }
}

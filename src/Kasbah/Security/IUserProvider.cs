using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Security.Models;

namespace Kasbah.Security
{
    public interface IUserProvider
    {
        Task UpdateLastLoginAsync(Guid id);

        Task<Guid> CreateUserAsync(string username, string password, string name = null, string email = null);

        Task<User> GetUserAsync(Guid id);

        Task<User> GetUserByUsernameAsync(string username);

        Task<IEnumerable<User>> ListUsersAsync();

        Task UpdateUserAsync(Guid id, string username, string password, string name = null, string email = null);
    }
}

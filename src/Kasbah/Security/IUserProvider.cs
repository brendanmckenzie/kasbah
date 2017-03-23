using System;
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
    }
}

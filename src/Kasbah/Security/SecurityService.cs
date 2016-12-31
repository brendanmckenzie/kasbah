using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.DataAccess;
using Kasbah.Security.Models;
using Microsoft.Extensions.Logging;

namespace Kasbah.Security
{
    public class SecurityService
    {
        const string IndexName = "security";
        readonly IDataAccessProvider _dataAccessProvider;
        readonly ILogger _log;

        public SecurityService(IDataAccessProvider dataAccessProvider, ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<SecurityService>();
            _dataAccessProvider = dataAccessProvider;
        }

        #region Public methods

        public async Task<User> VerifyUserAsync(string username, string password)
        {
            var user = await GetUserAsync(username);
            if (user != null)
            {
                // TODO: encrypt this... obviously.
                if (user.Password == password)
                {
                    // TODO: immutability
                    user.LastLogin = DateTime.UtcNow;

                    await _dataAccessProvider.PutEntryAsync(IndexName, user.Id, user);

                    return user;
                }
                else
                {
                    throw new InvalidLoginException();
                }
            }

            throw new UserNotFoundException();
        }

        public async Task<Guid> CreateUserAsync(string username, string password, string name = null, string email = null)
        {
            // Check user doesn't already exist
            if (await GetUserAsync(username) != null)
            {
                throw new UserAlreadyExistsException();
            }

            var id = Guid.NewGuid();
            var user = new User
            {
                Id = id,
                Username = username,
                Password = password, // TODO: ENCRYPT
                Name = name,
                Email = email,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            await _dataAccessProvider.PutEntryAsync(IndexName, id, user);

            return id;
        }

        public async Task<IEnumerable<User>> ListUsersAsync()
        {
            var entries = await _dataAccessProvider.QueryEntriesAsync<User>(IndexName, null);

            foreach (var entry in entries)
            {
                entry.Entry.Password = null;
            }

            return entries.Select(ent => ent.Entry);
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(SecurityService)}");
            await _dataAccessProvider.EnsureIndexExists(IndexName);

            // This isn't ideal, but we need to seed the data somehow/somewhere
            var adminUser = await GetUserAsync("admin");
            if (adminUser == null)
            {
                _log.LogDebug($"No admin user found, creating (username: admin, password: kasbah)");
                await CreateUserAsync("admin", "kasbah", "Administrator");
            }
        }

        #endregion

        #region Private methods

        async Task<User> GetUserAsync(string username)
        {
            var entries = await _dataAccessProvider.QueryEntriesAsync<User>(IndexName, new { term = new { Username = username } });

            return entries.FirstOrDefault()?.Entry;
        }

        #endregion
    }

    public class UserNotFoundException : Exception { }
    public class InvalidLoginException : Exception { }
    public class UserAlreadyExistsException : Exception { }
}
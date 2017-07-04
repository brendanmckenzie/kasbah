using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Kasbah.Security.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;

namespace Kasbah.Security
{
    public class SecurityService
    {
        readonly IUserProvider _userProvider;
        readonly ILogger _log;
        readonly int _iterCount = 4;
        readonly RandomNumberGenerator _rng;

        public SecurityService(ILogger<SecurityService> log, IUserProvider userProvider)
        {
            _log = log;
            _userProvider = userProvider;

            _rng = RandomNumberGenerator.Create();
        }

        public async Task<User> VerifyUserAsync(string username, string password)
        {
            var user = await GetUserAsync(username);
            if (user != null)
            {
                if (VerifyPassword(password, user.Password))
                {
                    user.LastLogin = DateTime.UtcNow;

                    await _userProvider.UpdateLastLoginAsync(user.Id);

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

            return await _userProvider.CreateUserAsync(username, EncryptPassword(password), name, email);
        }

        public async Task<IEnumerable<User>> ListUsersAsync()
            => await _userProvider.ListUsersAsync();

        public async Task<User> PutUserAsync(Guid id, string username, string password, string name = null, string email = null)
        {
            var user = await GetUserAsync(id);
            if (!string.Equals(user.Username, username))
            {
                var userByUsername = await GetUserAsync(username);
                if (userByUsername != null)
                {
                    throw new UserAlreadyExistsException();
                }
            }

            await _userProvider.UpdateUserAsync(id, username, string.IsNullOrEmpty(password) ? null : EncryptPassword(password), name, email);

            var ret = await GetUserAsync(id);

            // TODO: handle this in the provider
            ret.Password = null;

            return ret;
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(SecurityService)}");

            // This isn't ideal, but we need to seed the data somehow/somewhere
            var adminUser = await GetUserAsync("admin");
            if (adminUser == null)
            {
                _log.LogDebug($"No admin user found, creating (username: admin, password: kasbah)");
                await CreateUserAsync("admin", "kasbah", "Administrator");
            }
        }

        async Task<User> GetUserAsync(string username)
            => await _userProvider.GetUserByUsernameAsync(username);

        async Task<User> GetUserAsync(Guid id)
            => await _userProvider.GetUserAsync(id);

        string EncryptPassword(string input)
        {
            return Convert.ToBase64String(HashPassword(input));
        }

        bool VerifyPassword(string input, string hashed)
        {
            return PasswordUtil.VerifyHashedPassword(Convert.FromBase64String(hashed), input, out int iterCount);
        }

        byte[] HashPassword(string password)
        {
            return PasswordUtil.HashPassword(
                password: password,
                rng: _rng,
                prf: KeyDerivationPrf.HMACSHA256,
                iterCount: _iterCount,
                saltSize: 128 / 8,
                numBytesRequested: 256 / 8);
        }
    }
}

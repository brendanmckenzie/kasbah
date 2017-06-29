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

        #region Public methods

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
            if (!String.Equals(user.Username, username))
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

        #endregion

        #region Private methods

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
            int iterCount;
            return VerifyHashedPassword(Convert.FromBase64String(hashed), input, out iterCount);
        }

        // 'Borrowed' from ASP.NET Identity - PasswordHasher
        // https://github.com/aspnet/Identity/blob/5480aa182bad3fb3b729a0169d0462873331e306/src/Microsoft.AspNetCore.Identity/PasswordHasher.cs
        byte[] HashPassword(string password)
        {
            return HashPassword(password, _rng,
                prf: KeyDerivationPrf.HMACSHA256,
                iterCount: _iterCount,
                saltSize: 128 / 8,
                numBytesRequested: 256 / 8);
        }

        static byte[] HashPassword(string password, RandomNumberGenerator rng, KeyDerivationPrf prf, int iterCount, int saltSize, int numBytesRequested)
        {
            // Produce a version 3 (see comment above) text hash.
            byte[] salt = new byte[saltSize];
            rng.GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        static bool VerifyHashedPassword(byte[] hashedPassword, string password, out int iterCount)
        {
            iterCount = default(int);

            try
            {
                // Read header information
                KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
                iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
                int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }
                byte[] salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                int subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }
                byte[] expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the incoming password and verify it
                byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
                return ByteArraysEqual(actualSubkey, expectedSubkey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }

        static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }

        static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        #endregion
    }

    public class UserNotFoundException : Exception { }
    public class InvalidLoginException : Exception { }
    public class UserAlreadyExistsException : Exception { }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Kasbah.Security;
using Kasbah.Security.Models;
using Npgsql;

namespace Kasbah.Provider.Npgsql
{
    public class UserProvider : IUserProvider
    {
        readonly NpgsqlSettings _settings;

        public UserProvider(NpgsqlSettings settings)
        {
            _settings = settings;
        }

        public async Task UpdateLastLoginAsync(Guid id)
        {
            const string Sql = "update \"user\" set last_login_at = now() where id = @id";
            using (var connection = GetConnection())
            {
                await connection.ExecuteAsync(Sql, new { id });
            }
        }

        public async Task<Guid> CreateUserAsync(string username, string password, string name = null, string email = null)
        {
            const string Sql = "insert into \"user\" ( id, username, password, name, email ) values ( @id, @username, @password, @name, @email )";
            using (var connection = GetConnection())
            {
                var id = Guid.NewGuid();

                await connection.ExecuteAsync(Sql, new { id, username, password, name, email });

                return id;
            }
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            const string Sql = "select id as Id, username as Username, password as Password, name as Name, email as Email, created_at as Created, modified_at as Modified, last_login_at as LastLogin from \"user\" where id = @id";
            using (var connection = GetConnection())
            {
                return await connection.QuerySingleAsync<User>(Sql, new { id });
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            const string Sql = "select id as Id, username as Username, password as Password, name as Name, email as Email, created_at as Created, modified_at as Modified, last_login_at as LastLogin from \"user\" where username = @username";
            using (var connection = GetConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<User>(Sql, new { username });
            }
        }

        public async Task<IEnumerable<User>> ListUsersAsync()
        {
            const string Sql = "select id as Id, username as Username, name as Name, email as Email, created_at as Created, modified_at as Modified, last_login_at as LastLogin from \"user\" order by last_login_at desc";
            using (var connection = GetConnection())
            {
                return await connection.QueryAsync<User>(Sql);
            }
        }

        public async Task UpdateUserAsync(Guid id, string username, string password, string name = null, string email = null)
        {
            const string Sql = "update \"user\" set username = :username, password = coalesce(:password, password), name = name, email = :email where id = :id";
            using (var connection = GetConnection())
            {
                await connection.ExecuteAsync(Sql, new { id, username, password, name, email });
            }
        }

        NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_settings.ConnectionString);
        }
    }
}

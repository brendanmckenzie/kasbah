using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using Kasbah.Provider.Npgsql.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;

namespace Kasbah.Provider.Npgsql
{
    internal class DatabaseUtility
    {
        readonly ILogger<DatabaseUtility> _log;

        public DatabaseUtility(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<DatabaseUtility>();
        }

        public void InitialiseSchema(NpgsqlSettings settings)
        {
            _log.LogInformation("Checking database is initialised");
            using (var connection = settings.GetConnection())
            {
                connection.Open();
                var initialised = CheckDatabaseInitialised(connection);
                _log.LogInformation($"Database is{(initialised ? string.Empty : " not")} initialised");
                if (!initialised)
                {
                    _log.LogInformation($"Initialising database");
                    var initTransaction = connection.BeginTransaction();
                    connection.Execute("create schema kasbah;");
                    connection.Execute("create table kasbah.setting ( key text, value jsonb );");
                    connection.Execute("insert into kasbah.setting values ( 'schema_version', '{\"version\":0}'::jsonb );");
                    initTransaction.Commit();
                    _log.LogInformation($"Database initialised");
                }

                _log.LogInformation("Checking for pending database migrations");
                var currentSchemaVersion = GetSystemValue<SchemaVersion>(connection, SchemaVersion.Key);
                var assembly = typeof(DatabaseUtility).GetTypeInfo().Assembly;
                const string Prefix = "Kasbah.Provider.Npgsql.Resources.migration_";
                var resources = assembly.GetManifestResourceNames()
                    .Where(ent => ent.StartsWith(Prefix))
                    .Select(ent => new
                    {
                        Name = ent,
                        ShortName = ent.Replace(".sql", string.Empty).Split('.').Last(),
                        Version = int.Parse(ent
                            .Replace(Prefix, string.Empty)
                            .Replace(".sql", string.Empty)
                            .TrimStart(new[] { '0' }))
                    })
                    .Where(ent => ent.Version > currentSchemaVersion.Version)
                    .OrderBy(ent => ent.Version);

                if (resources.Any())
                {
                    _log.LogInformation($"Pending database migrations: {string.Join(", ", resources.Select(ent => ent.ShortName))}");
                    var transaction = connection.BeginTransaction();
                    try
                    {
                        foreach (var resource in resources)
                        {
                            _log.LogInformation($"Running migrations in '{resource.ShortName}'");
                            using (var stream = assembly.GetManifestResourceStream(resource.Name))
                            {
                                var buffer = new byte[stream.Length];

                                stream.Read(buffer, 0, buffer.Length);

                                var content = Encoding.UTF8.GetString(buffer);

                                var statements = content.Split(';')
                                    .Where(ent => !string.IsNullOrWhiteSpace(ent));

                                foreach (var sql in statements)
                                {
                                    try
                                    {
                                        connection.Execute(sql);
                                    }
                                    catch (PostgresException ex)
                                    {
                                        _log.LogError($"Migration failed: '{resource.ShortName}'; statement: {sql}; error: {ex.Message}");
                                        throw;
                                    }
                                }

                                PutSystemValue(connection, SchemaVersion.Key, new SchemaVersion { Version = resource.Version });

                                _log.LogInformation($"Migration run successfully: '{resource.ShortName}'");
                            }
                        }

                        transaction.Commit();

                        _log.LogInformation("All migrations run successfully");
                    }
                    catch (PostgresException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                else
                {
                    _log.LogInformation("No database migrations pending");
                }
            }
        }

        public bool CheckDatabaseInitialised(NpgsqlConnection connection)
            => connection.Query<bool>("select count(*) = 1 from pg_catalog.pg_tables where schemaname = 'kasbah' and tablename = 'setting'").First();

        public T GetSystemValue<T>(NpgsqlConnection connection, string key)
        {
            var json = connection.Query<string>("select value from kasbah.setting where key = @key", new { key }).FirstOrDefault();

            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        public void PutSystemValue<T>(NpgsqlConnection connection, string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);

            connection.Execute("update kasbah.setting set value = @json::jsonb where key = @key", new { key, json });
        }
    }
}

using System.Data.Common;
using Npgsql;
using StackExchange.Profiling;

namespace Kasbah.Provider.Npgsql
{
    public static class ConnectionExtensions
    {
        internal static DbConnection GetConnection(this NpgsqlSettings settings)
        {
            var connection = new NpgsqlConnection(settings.ConnectionString);

            return connection;

            // return new StackExchange.Profiling.Data.ProfiledDbConnection(connection, MiniProfiler.Current);
        }
    }
}

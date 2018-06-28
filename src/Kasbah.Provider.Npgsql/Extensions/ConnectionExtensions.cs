using Npgsql;

namespace Kasbah.Provider.Npgsql
{
    public static class ConnectionExtensions
    {
        internal static NpgsqlConnection GetConnection(this NpgsqlSettings settings)
            => new NpgsqlConnection(settings.ConnectionString);
    }
}

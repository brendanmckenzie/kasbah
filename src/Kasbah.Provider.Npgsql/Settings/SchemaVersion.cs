namespace Kasbah.Provider.Npgsql.Settings
{
    public class SchemaVersion
    {
        public const string Key = "schema_version";

        public int Version { get; set; }
    }
}

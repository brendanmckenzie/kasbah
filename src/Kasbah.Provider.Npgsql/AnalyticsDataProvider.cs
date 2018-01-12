using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Kasbah.Analytics;
using Kasbah.Analytics.Models;
using Newtonsoft.Json;
using Npgsql;

namespace Kasbah.Provider.Npgsql
{
    public class AnalyticsDataProvider : IAnalyticsDataProvider
    {
        readonly NpgsqlSettings _settings;

        public AnalyticsDataProvider(NpgsqlSettings settings)
        {
            _settings = settings;
        }

        public Task CreateAttributeAsync(Guid profile, string alias, object data)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateCampaignAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateProfileAsync()
        {
            throw new NotImplementedException();
        }

        public async Task CreateSessionAsync(Guid id)
        {
            const string Sql = @"insert into session ( id ) values ( @id )";

            using (var connection = GetConnection())
            {
                await connection.ExecuteAsync(Sql, new { id });
            }
        }

        public Task<Profile> GetProfileAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Campaign> ListCampaignsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Profile>> ListProfiles(int? skip, int? take, IEnumerable<string> attributes)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Profile>> ListProfilesByAttribute(KeyValuePair<string, object> attribute, int? skip, int? take, IEnumerable<string> attributes)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Profile>> ListProfilesByCampaign(string campaign, int? skip, int? take, IEnumerable<string> attributes)
        {
            throw new NotImplementedException();
        }

        public Task TrackEventAsync(Guid profile, string @event, string source, object data, string campaign, Guid? node, int? version)
        {
            throw new NotImplementedException();
        }

        public async Task TrackSessionActivityAsync(Guid session, string type, object data)
        {
            const string Sql = @"
insert into session_activity ( id, session_id, type, attributes ) values ( @id, @session, @type, @data::jsonb );
update session set last_activity_at = now() where id = @session;";

            using (var connection = GetConnection())
            {
                var id = Guid.NewGuid();
                await connection.ExecuteAsync(Sql, new { id, session, type, data = JsonConvert.SerializeObject(data) });
            }
        }

        NpgsqlConnection GetConnection()
            => new NpgsqlConnection(_settings.ConnectionString);
    }
}

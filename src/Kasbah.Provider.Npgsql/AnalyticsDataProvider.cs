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

            using (var connection = _settings.GetConnection())
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
insert into session ( id ) values ( @id ) on conflict ( id ) do update set last_activity_at = now();
insert into session_activity ( id, session_id, type, attributes ) values ( @id, @session, @type, @data::jsonb );";

            using (var connection = _settings.GetConnection())
            {
                var id = Guid.NewGuid();
                await connection.ExecuteAsync(Sql, new { id, session, type, data = JsonConvert.SerializeObject(data) });
            }
        }

        public async Task<IEnumerable<ReportingData>> ListSessionActivityReportingAsync(string type, string interval, DateTime start, DateTime end)
        {
            const string Sql = @"
select
  date_trunc(@interval, created_at) as Period,
  count(*) as ""Count""
from
  session_activity
where
  type = @type
  and created_at between @start and @end
group by
  date_trunc(@interval, created_at)
order by
  date_trunc(@interval, created_at);";

            using (var connection = _settings.GetConnection())
            {
                return await connection.QueryAsync<ReportingData>(Sql, new { type, interval, start, end });
            }
        }

        public async Task<IEnumerable<ReportingData>> ListSessionReportingAsync(string interval, DateTime start, DateTime end)
        {
            const string Sql = @"
select
  date_trunc(@interval, last_activity_at) as Period,
  count(*) as ""Count""
from
  session
where
  last_activity_at between @start and @end
group by
  date_trunc(@interval, last_activity_at)
order by
  date_trunc(@interval, last_activity_at);";

            using (var connection = _settings.GetConnection())
            {
                return await connection.QueryAsync<ReportingData>(Sql, new { interval, start, end });
            }
        }

        public async Task<IEnumerable<Session>> ListSessionsAsync(int skip, int take)
        {
            const string Sql = @"
select
    id as Id,
    attributes as Attributes,
    created_at as Created,
    last_activity_at as LastActivity
from
    session
order by
    last_activity_at desc
limit
    @take
offset
    @skip;";

            using (var connection = _settings.GetConnection())
            {
                return await connection.QueryAsync<Session>(Sql, new { skip, take });
            }
        }

        public async Task<IEnumerable<SessionActivity>> ListSessionActivityAsync(Guid session, int skip, int take, string type = null)
        {
            const string Sql = @"
select
    id as Id,
    session_id as SessionId,
    type as Type,
    attributes as Attributes,
    created_at as Created
from
    session_activity
where
    session_id = @session
    and (@type is null
    or type = @type)
order by
    created_at desc
limit
    @take
offset
    @skip;";

            using (var connection = _settings.GetConnection())
            {
                return await connection.QueryAsync<SessionActivity>(Sql, new { session, skip, take, type });
            }
        }
    }
}

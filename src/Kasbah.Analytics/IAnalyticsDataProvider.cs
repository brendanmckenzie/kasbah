using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Analytics.Models;

namespace Kasbah.Analytics
{
    public interface IAnalyticsDataProvider
    {
        Task<Guid> CreateProfileAsync();

        Task TrackEventAsync(Guid profile, string @event, string source, object data, string campaign, Guid? node, int? version);

        Task TrackSessionActivityAsync(Guid session, string type, object data);

        Task CreateSessionAsync(Guid id);

        Task CreateAttributeAsync(Guid profile, string alias, object data);

        Task<IEnumerable<Profile>> ListProfiles(int? skip, int? take, IEnumerable<string> attributes);

        Task<IEnumerable<Profile>> ListProfilesByAttribute(KeyValuePair<string, object> attribute, int? skip, int? take, IEnumerable<string> attributes);

        Task<IEnumerable<Profile>> ListProfilesByCampaign(string campaign, int? skip, int? take, IEnumerable<string> attributes);

        Task<Profile> GetProfileAsync(Guid id);

        Task<Guid> CreateCampaignAsync(string name);

        Task<Campaign> ListCampaignsAsync();

        Task<IEnumerable<ReportingData>> ListSessionActivityReportingAsync(string type, string interval, DateTime start, DateTime end);

        Task<IEnumerable<ReportingData>> ListSessionReportingAsync(string interval, DateTime start, DateTime end);

        Task<IEnumerable<Session>> ListSessionsAsync(int skip, int take);

        Task<IEnumerable<SessionActivity>> ListSessionActivityAsync(Guid session, int skip, int take, string type = null);
    }
}

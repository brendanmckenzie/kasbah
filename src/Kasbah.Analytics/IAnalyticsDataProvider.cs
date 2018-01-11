using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Analytics.Models;

namespace Kasbah.Analytics
{
    public interface IAnalyticsDataProvider
    {
         Task<Guid> CreateProfileAsync();

         Task TrackEventAsync(Guid profile, string @event, object data, string campaign, Guid? node, int? version);

         Task CreateAttributeAsync(Guid profile, string alias, object data);

         Task<IEnumerable<Profile>> ListProfiles(int? skip, int? take, IEnumerable<string> attributes);

         Task<IEnumerable<Profile>> ListProfilesByAttribute(KeyValuePair<string, object> attribute, int? skip, int? take, IEnumerable<string> attributes);

         Task<IEnumerable<Profile>> ListProfilesByCampaign(string campaign, int? skip, int? take, IEnumerable<string> attributes);

         Task<Profile> GetProfileAsync(Guid id);

         Task<Guid> CreateCampaignAsync(string name);

         Task<Campaign> ListCampaignsAsync();
    }
}

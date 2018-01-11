using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Analytics.Models;

namespace Kasbah.Analytics
{
    public class ReportingService
    {
        readonly IAnalyticsDataProvider _dataProvider;

        public ReportingService(IAnalyticsDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<IEnumerable<Profile>> ListProfiles(int? skip = null, int? take = null, IEnumerable<string> attributes = null)
            => await _dataProvider.ListProfiles(skip, take, attributes);

        public async Task<IEnumerable<Profile>> ListProfilesByAttribute(KeyValuePair<string, object> attribute, int? skip = null, int? take = null, IEnumerable<string> attributes = null)
            => await _dataProvider.ListProfilesByAttribute(attribute, skip, take, attributes);

        public async Task<IEnumerable<Profile>> ListProfilesByCampaign(string campaign, int? skip = null, int? take = null, IEnumerable<string> attributes = null)
            => await _dataProvider.ListProfilesByCampaign(campaign, skip, take, attributes);

        public async Task<Profile> GetProfileAsync(Guid id)
            => await _dataProvider.GetProfileAsync(id);
    }
}

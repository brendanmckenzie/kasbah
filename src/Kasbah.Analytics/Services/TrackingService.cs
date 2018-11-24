using System;
using System.Threading.Tasks;

namespace Kasbah.Analytics.Services
{
    public class TrackingService
    {
        const string TrackCampaignEvent = "campaign";

        readonly IAnalyticsDataProvider _dataProvider;

        public TrackingService(IAnalyticsDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task TrackEventAsync(Guid profile, string @event, string source = null, object data = null, string campaign = null, Guid? node = null, int? version = null)
            => await _dataProvider.TrackEventAsync(profile, @event, source, data, campaign, node, version);

        public async Task TrackCampaignAsync(Guid profile, string campaign)
            => await TrackEventAsync(profile, TrackCampaignEvent, null, campaign);

        public async Task CreateAttributeAsync(Guid profile, string alias, object data)
            => await _dataProvider.CreateAttributeAsync(profile, alias, data);

        public async Task TrackSessionActivityAsync(Guid session, string type, object data)
            => await _dataProvider.TrackSessionActivityAsync(session, type, data);
    }
}

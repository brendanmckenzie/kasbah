using System;
using System.Threading.Tasks;
using Kasbah.Analytics.Models;

namespace Kasbah.Analytics
{
    public class ManagementService
    {
        readonly IAnalyticsDataProvider _dataProvider;

        public ManagementService(IAnalyticsDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<Guid> CreateCampaignAsync(string name)
            => await _dataProvider.CreateCampaignAsync(name);

        public async Task<Campaign> ListCampaignsAsync()
            => await _dataProvider.ListCampaignsAsync();
    }
}

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
            => _dataProvider.CreateCampaignAsync(name);

        public async Task<Campaign> ListCampaignsAsync()
            => _dataProvider.ListCampaignsAsync();
    }
}

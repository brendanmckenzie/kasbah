using System;
using System.Threading.Tasks;

namespace Kasbah.Analytics.Services
{
    public class SessionService
    {
        readonly IAnalyticsDataProvider _dataProvider;

        public SessionService(IAnalyticsDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task CreateSessionAsync(Guid id)
            => await _dataProvider.CreateSessionAsync(id);
    }
}

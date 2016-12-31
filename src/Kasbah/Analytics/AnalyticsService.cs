using System;
using System.Threading.Tasks;
using Kasbah.Analytics.Models;
using Kasbah.DataAccess;

namespace Kasbah.Analytics
{
    public class AnalyticsService
    {
        const string IndexName = "analytics";
        readonly IDataAccessProvider _dataAccessProvider;
        public AnalyticsService(IDataAccessProvider dataAccessProvider)
        {
            _dataAccessProvider = dataAccessProvider;
        }

        public async Task TrackEvent(AnalyticsEvent ev)
        {
            await _dataAccessProvider.PutEntryAsync(IndexName, Guid.NewGuid(), ev);
        }

        // TODO: add reporting functionality
    }
}
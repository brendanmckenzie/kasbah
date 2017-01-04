using System;
using System.Threading.Tasks;
using Kasbah.Analytics.Models;
using Kasbah.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kasbah.Analytics
{
    public class AnalyticsService
    {
        const string IndexName = "analytics";
        readonly IDataAccessProvider _dataAccessProvider;
        readonly ILogger _log;
        public AnalyticsService(ILoggerFactory loggerFactory, IDataAccessProvider dataAccessProvider)
        {
            _log = loggerFactory.CreateLogger<AnalyticsService>();
            _dataAccessProvider = dataAccessProvider;
        }

        public async Task TrackEvent(AnalyticsEvent ev)
        {
            await _dataAccessProvider.PutEntryAsync(IndexName, Guid.NewGuid(), ev);
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(AnalyticsService)}");
            await _dataAccessProvider.EnsureIndexExists(IndexName);
        }

        // TODO: add reporting functionality
    }
}
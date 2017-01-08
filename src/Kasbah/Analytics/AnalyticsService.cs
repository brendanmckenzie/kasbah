using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Analytics.Models;
using Kasbah.DataAccess;
using Microsoft.Extensions.Logging;

namespace Kasbah.Analytics
{
    public class AnalyticsService
    {
        static class Indicies
        {
            public const string Analytics = "analytics";
            public const string Personas = "personas";
        }

        readonly IDataAccessProvider _dataAccessProvider;
        readonly ILogger _log;
        public AnalyticsService(ILoggerFactory loggerFactory, IDataAccessProvider dataAccessProvider)
        {
            _log = loggerFactory.CreateLogger<AnalyticsService>();
            _dataAccessProvider = dataAccessProvider;
        }

        public async Task TrackEvent(AnalyticsEvent ev)
        {
            await _dataAccessProvider.PutEntryAsync(Indicies.Analytics, Guid.NewGuid(), ev);
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(AnalyticsService)}");
            await _dataAccessProvider.EnsureIndexExists(Indicies.Analytics);
        }

        public async Task<IEnumerable<AnalyticsEvent>> EventDumpAsync()
        {
            var entries = await _dataAccessProvider.QueryEntriesAsync<AnalyticsEvent>(Indicies.Analytics);

            return entries.Select(ent => ent.Source);
        }

        public async Task<Guid> CreatePersonaAsync()
        {
            var id = Guid.NewGuid();

            var persona = new Persona
            {
                Id = id
            };

            await _dataAccessProvider.PutEntryAsync(Indicies.Personas, id, persona);

            return id;
        }

        public async Task MergePersonasAsync(Guid source, Guid dest)
        {
            throw await Task.FromResult(new NotImplementedException());
        }

        // TODO: add reporting functionality
    }
}
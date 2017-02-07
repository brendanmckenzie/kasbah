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
            public const string Profiles = "profiles";
            public const string Attributes = "attributes";
            public const string Traits = "traits";
            public const string Events = "events";
        }

        readonly IDataAccessProvider _dataAccessProvider;
        readonly ILogger _log;
        readonly AnalyticsBus _analyticsBus;

        public AnalyticsService(ILoggerFactory loggerFactory, IDataAccessProvider dataAccessProvider, AnalyticsBus analyticsBus)
        {
            _log = loggerFactory.CreateLogger<AnalyticsService>();
            _dataAccessProvider = dataAccessProvider;
            _analyticsBus = analyticsBus;
        }

        public async Task TrackEvent(AnalyticsEvent ev)
        {
            await _dataAccessProvider.PutEntryAsync(Indicies.Events, Guid.NewGuid(), ev, waitForCommit: false);

            var profile = await GetProfile(ev.Profile);

            foreach (var processor in _analyticsBus.Processors)
            {
                await processor.HandleEvent(profile, ev,
                    (key, value) =>
                    {
                        var attribute = new ProfileAttribute
                        {
                            Profile = profile.Id,
                            Key = key,
                            Value = value
                        };

                        _dataAccessProvider.PutEntryAsync(Indicies.Attributes, Guid.NewGuid(), attribute, waitForCommit: false);
                    },
                    (key, weight) =>
                    {
                        var trait = new ProfileTrait
                        {
                            Profile = profile.Id,
                            Key = key,
                            Value = weight
                        };

                        _dataAccessProvider.PutEntryAsync(Indicies.Traits, Guid.NewGuid(), trait, waitForCommit: false);
                    });
            }
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(AnalyticsService)}");
            await _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Analytics);
            await _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Profiles);
            await _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Events);
            await _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Attributes);
            await _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Traits);
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

        public async Task<Profile> GetProfile(Guid id)
        {
            var entry = await _dataAccessProvider.GetEntryAsync<Profile>(Indicies.Profiles, id);

            var ret = entry.Source;

            ret.Attributes = (await _dataAccessProvider.QueryEntriesAsync<ProfileAttribute>(Indicies.Attributes, new { query = new { match = new { Profile = id } } })).Select(ent => ent.Source);
            ret.Traits = (await _dataAccessProvider.QueryEntriesAsync<ProfileTrait>(Indicies.Traits, new { query = new { match = new { Profile = id } } })).Select(ent => ent.Source);

            return ret;
        }

        // TODO: add reporting functionality
    }
}

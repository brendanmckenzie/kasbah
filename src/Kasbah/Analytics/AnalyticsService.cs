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
            public const string Profiles = "analytics_profiles";
            public const string Attributes = "analytics_attributes";
            public const string Traits = "analytics_traits";
            public const string Events = "analytics_events";
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

        public async Task TrackEventAsync(AnalyticsEvent ev)
        {
            await _dataAccessProvider.PutEntryAsync(Indicies.Events, Guid.NewGuid(), ev, waitForCommit: false);

            var profile = await GetProfileAsync(ev.Profile);

            foreach (var processor in _analyticsBus.Processors)
            {
                await processor.HandleEvent(profile, ev,
                    async (key, value) =>
                    {
                        var attribute = new ProfileAttribute
                        {
                            Profile = ev.Profile,
                            Key = key,
                            Value = value
                        };

                        await _dataAccessProvider.PutEntryAsync(Indicies.Attributes, Guid.NewGuid(), attribute, waitForCommit: false);
                    },
                    async (key, weight) =>
                    {
                        var trait = new ProfileTrait
                        {
                            Profile = ev.Profile,
                            Key = key,
                            Value = weight
                        };

                        await _dataAccessProvider.PutEntryAsync(Indicies.Traits, Guid.NewGuid(), trait, waitForCommit: false);
                    });
            }
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(AnalyticsService)}");

            await Task.WhenAll(
                _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Profiles),
                _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Events),
                _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Attributes),
                _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Traits),
                UpdateMappingsAsync());
        }

        public async Task<Profile> GetProfileAsync(Guid id)
        {
            var profile = default(Profile);
            try
            {
                var entry = await _dataAccessProvider.GetEntryAsync<Profile>(Indicies.Profiles, id);
                profile = entry.Source;
            }
            catch
            {
                profile = new Profile { Id = id };
                await _dataAccessProvider.PutEntryAsync<Profile>(Indicies.Profiles, profile.Id, profile);
            }

            profile.Attributes = (await _dataAccessProvider.QueryEntriesAsync<ProfileAttribute>(Indicies.Attributes, new { match = new { Profile = id } })).Select(ent => ent.Source);
            profile.Traits = (await _dataAccessProvider.QueryEntriesAsync<ProfileTrait>(Indicies.Traits, new { match = new { Profile = id } })).Select(ent => ent.Source);
            profile.Events = (await _dataAccessProvider.QueryEntriesAsync<AnalyticsEvent>(Indicies.Events, new { match = new { Profile = id } })).Select(ent => ent.Source);

            return profile;
        }

        public async Task<IEnumerable<ProfileSummary>> ListProfilesAsync()
        {
            var entries = await _dataAccessProvider.QueryEntriesAsync<Profile>(Indicies.Profiles);

            return entries.Select(ent => ent.Source);
        }

        async Task UpdateMappingsAsync()
        {
            await Task.WhenAll(
                UpdateProfileMappingAsync(),
                UpdateEventMappingAsync(),
                UpdateAttributeMappingAsync(),
                UpdateTraitMappingAsync());
        }

        async Task UpdateProfileMappingAsync()
        {
            var properties = new Dictionary<string, object> {
                { nameof(Profile.Id), new { type = "keyword" } },
                { nameof(Profile.Created), new { type = "date" } }
            };

            await _dataAccessProvider.PutTypeMappingAsync(Indicies.Profiles, typeof(Profile), new { properties });
        }

        async Task UpdateEventMappingAsync()
        {
            var properties = new Dictionary<string, object> {
                { nameof(AnalyticsEvent.Profile), new { type = "keyword" } },
                { nameof(AnalyticsEvent.Created), new { type = "date" } },
                { nameof(AnalyticsEvent.Source), new { type = "keyword" } },
                { nameof(AnalyticsEvent.Type), new { type = "keyword" } },
                { nameof(AnalyticsEvent.Data), new { dynamic = true, properties = new object() } }
            };

            await _dataAccessProvider.PutTypeMappingAsync(Indicies.Events, typeof(AnalyticsEvent), new { properties });
        }

        async Task UpdateAttributeMappingAsync()
        {
            var properties = new Dictionary<string, object> {
                { nameof(ProfileAttribute.Profile), new { type = "keyword" } },
                { nameof(ProfileAttribute.Created), new { type = "date" } },
                { nameof(ProfileAttribute.Key), new { type = "keyword" } },
                { nameof(ProfileAttribute.Value), new { type = "text" } }
            };

            await _dataAccessProvider.PutTypeMappingAsync(Indicies.Attributes, typeof(ProfileAttribute), new { properties });
        }

        async Task UpdateTraitMappingAsync()
        {
            var properties = new Dictionary<string, object> {
                { nameof(ProfileTrait.Profile), new { type = "keyword" } },
                { nameof(ProfileTrait.Created), new { type = "date" } },
                { nameof(ProfileTrait.Key), new { type = "keyword" } },
                { nameof(ProfileTrait.Value), new { type = "long" } }
            };

            await _dataAccessProvider.PutTypeMappingAsync(Indicies.Traits, typeof(ProfileTrait), new { properties });
        }
    }
}

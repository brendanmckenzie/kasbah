using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Analytics.Models;
using Kasbah.Content.Models;
using Kasbah.DataAccess;
using Kasbah.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Kasbah.Content;
using System.Reflection;
using Kasbah.Exceptions;

namespace Kasbah.Analytics
{
    public class AnalyticsService
    {
        static class Indicies
        {
            public const string Profiles = "analytics_profiles";
            public const string Attributes = "analytics_attributes";
            public const string Bias = "analytics_bias";
            public const string Events = "analytics_events";
        }

        readonly ILogger _log;
        readonly IDataAccessProvider _dataAccessProvider;
        readonly ContentService _contentService;
        readonly TypeMapper _typeMapper;
        readonly IDistributedCache _cache;

        public AnalyticsService(ILoggerFactory loggerFactory, IDataAccessProvider dataAccessProvider, ContentService contentService, TypeMapper typeMapper, IDistributedCache cache = null)
        {
            _log = loggerFactory.CreateLogger<AnalyticsService>();
            _dataAccessProvider = dataAccessProvider;
            _contentService = contentService;
            _typeMapper = typeMapper;
            _cache = cache;
        }

        public async Task TrackEventAsync(Guid profile, string type, string source, IDictionary<string, string> data)
        {
            var ev = new AnalyticsEvent
            {
                Profile = profile,
                Type = type,
                Source = source,
                Data = data
            };
            await _dataAccessProvider.PutEntryAsync(Indicies.Events, Guid.NewGuid(), ev, waitForCommit: false);

            await UpdateProfileLastActivityAsync(profile);

            await _cache.RemoveAsync(ProfileCacheKey(profile));
        }

        public async Task TriggerBiasAsync(Guid profile, string bias, long weight)
        {
            var attr = new ProfileBias
            {
                Profile = profile,
                Key = bias,
                Value = weight
            };

            await _dataAccessProvider.PutEntryAsync(Indicies.Bias, Guid.NewGuid(), attr, waitForCommit: false);

            await UpdateProfileLastActivityAsync(profile);

            await _cache.RemoveAsync(ProfileCacheKey(profile));
        }

        public async Task SetAttributeAsync(Guid profile, string attribute, string value, bool clearCache = true)
        {
            var attr = new ProfileAttribute
            {
                Profile = profile,
                Key = attribute,
                Value = value
            };

            await _dataAccessProvider.PutEntryAsync(Indicies.Attributes, Guid.NewGuid(), attr, waitForCommit: false);

            if (clearCache)
            {
                await UpdateProfileLastActivityAsync(profile);

                await _cache.RemoveAsync(ProfileCacheKey(profile));
            }
        }

        public async Task SetAttributesAsync(Guid profile, IDictionary<string, string> values)
        {
            await Task.WhenAll(values.Select(async ent => await SetAttributeAsync(profile, ent.Key, ent.Value, false)));

            await UpdateProfileLastActivityAsync(profile);

            await _cache.RemoveAsync(ProfileCacheKey(profile));
        }

        public async Task InitialiseAsync()
        {
            _log.LogDebug($"Initialising {nameof(AnalyticsService)}");

            await Task.WhenAll(
                _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Profiles),
                _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Events),
                _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Attributes),
                _dataAccessProvider.EnsureIndexExistsAsync(Indicies.Bias));

            await UpdateMappingsAsync();
        }

        public async Task<Profile> GetProfileAsync(Guid id)
        {
            return await _cache.GetOrSetAsync(ProfileCacheKey(id), async () =>
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

                profile.Attributes = (await _dataAccessProvider.QueryEntriesAsync<ProfileAttribute>(Indicies.Attributes, new { match = new { Profile = id } }, take: 1024)).Select(ent => ent.Source);
                profile.Bias = (await _dataAccessProvider.QueryEntriesAsync<ProfileBias>(Indicies.Bias, new { match = new { Profile = id } }, take: 1024)).Select(ent => ent.Source);
                profile.Events = (await _dataAccessProvider.QueryEntriesAsync<AnalyticsEvent>(Indicies.Events, new { match = new { Profile = id } }, take: 1024)).Select(ent => ent.Source);

                return profile;
            });
        }

        public async Task<IEnumerable<ProfileSummary>> ListProfilesAsync()
        {
            var entries = await _dataAccessProvider.QueryEntriesAsync<Profile>(Indicies.Profiles, take: 1024);

            return entries.Select(ent => ent.Source);
        }

        public async Task<object> PatchContentAsync(Guid profile, Node node, object content, TypeDefinition type)
        {
            var profileObj = await GetProfileAsync(profile);
            var patches = await _contentService.ListContentPatchesAsync(node.Id);

            // TODO: calculate order of precedence on matching patches
            var applicablePatches = patches.Where(patch =>
                patch.Attributes.Any(patchAttr =>
                {
                    var profileAttr = profileObj.GetAttributeValue(patchAttr.Key);
                    if (!string.IsNullOrEmpty(profileAttr))
                    {
                        return profileAttr == patchAttr.Value;
                    }

                    return false;
                })
                ||
                patch.Bias.Any(patchBias =>
                {
                    var profileBias = profileObj.Bias.Where(bias => bias.Key == patchBias.Key).Sum(bias => bias.Value);

                    return profileBias > patchBias.Value;
                })
            );

            if (applicablePatches.Any())
            {
                var contentType = content.GetType().GetTypeInfo();

                foreach (var patch in applicablePatches)
                {
                    foreach (var kvp in patch.Values)
                    {
                        var prop = contentType.GetProperty(kvp.Key);
                        if (prop != null)
                        {
                            var value = await _typeMapper.MapPropertyAsync(kvp.Value, prop);

                            prop.SetValue(content, value);
                        }
                    }
                }
            }

            return await Task.FromResult(content);
        }

        async Task UpdateMappingsAsync()
        {
            await Task.WhenAll(
                UpdateProfileMappingAsync(),
                UpdateEventMappingAsync(),
                UpdateAttributeMappingAsync(),
                UpdateBiasMappingAsync());
        }

        async Task UpdateProfileMappingAsync()
        {
            var properties = new Dictionary<string, object> {
                { nameof(Profile.Id), new { type = "keyword" } },
                { nameof(Profile.Created), new { type = "date" } },
                { nameof(Profile.LastActivity), new { type = "date" } }
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

        async Task UpdateBiasMappingAsync()
        {
            var properties = new Dictionary<string, object> {
                { nameof(ProfileBias.Profile), new { type = "keyword" } },
                { nameof(ProfileBias.Created), new { type = "date" } },
                { nameof(ProfileBias.Key), new { type = "keyword" } },
                { nameof(ProfileBias.Value), new { type = "long" } }
            };

            await _dataAccessProvider.PutTypeMappingAsync(Indicies.Bias, typeof(ProfileBias), new { properties });
        }

        async Task UpdateProfileLastActivityAsync(Guid id)
        {
            var profile = default(Profile);

            try
            {
                var entry = await _dataAccessProvider.GetEntryAsync<Profile>(Indicies.Profiles, id);

                profile = entry.Source;
            }
            catch (EntryNotFoundException)
            {
                profile = new Profile
                {
                    Id = id,
                    Created = DateTime.UtcNow
                };
            }

            profile.LastActivity = DateTime.UtcNow;

            await _dataAccessProvider.PutEntryAsync(Indicies.Profiles, id, profile);
        }

        string ProfileCacheKey(Guid profile) => $"{nameof(Profile)}_{profile}";
    }
}

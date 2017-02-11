using System;
using System.Collections.Generic;
using System.Linq;

namespace Kasbah.Analytics.Models
{
    public class ProfileSummary
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; }
    }

    public class Profile : ProfileSummary
    {
        public IEnumerable<ProfileAttribute> Attributes { get; set; }
        public IEnumerable<ProfileBias> Bias { get; set; }
        public IEnumerable<AnalyticsEvent> Events { get; set; }

        public string GetAttributeValue(string key)
        {
            return (Attributes ?? Enumerable.Empty<ProfileAttribute>())
                .OrderByDescending(ent => ent.Created)
                .FirstOrDefault()
                ?.Value;
        }

        public string this[string attributeKey]
            => GetAttributeValue(attributeKey);
    }

    public class ProfileAttribute
    {
        public Guid Profile { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }

    public class ProfileBias
    {
        public Guid Profile { get; set; }
        public string Key { get; set; }
        public long Value { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.Collections.Generic;

namespace Kasbah.Analytics.Models
{
    public class Profile
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public IEnumerable<ProfileAttribute> Attributes { get; set; }
        public IEnumerable<ProfileTrait> Traits { get; set; }
    }

    public class ProfileAttribute
    {
        public Guid Profile { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }

    public class ProfileTrait
    {
        public Guid Profile { get; set; }
        public string Key { get; set; }
        public long Value { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}

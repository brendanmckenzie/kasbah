using System;
using System.Text;

namespace Kasbah.Media
{
    public class GetMediaRequest
    {
        public Guid Id { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public bool IsEmpty
            => !Width.HasValue && !Height.HasValue;

        internal string Hash
            => Convert.ToBase64String(Encoding.UTF8.GetBytes($"Id={Id}&Width={Width}&Height={Height}"));
    }
}

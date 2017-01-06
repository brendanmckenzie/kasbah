using System;

namespace Kasbah.Media.Models
{
    public class MediaItem
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
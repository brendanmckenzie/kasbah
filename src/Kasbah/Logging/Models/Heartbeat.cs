using System;

namespace Kasbah.Logging.Models
{
    public class Heartbeat
    {
        public Guid Instance { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
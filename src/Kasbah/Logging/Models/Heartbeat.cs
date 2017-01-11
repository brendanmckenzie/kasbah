using System;

namespace Kasbah.Logging.Models
{
    public class Heartbeat
    {
        public Guid Instance { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public ulong RequestsTotal { get; set; } = 0;
        public ulong RequestsLatest { get; set; } = 0;
    }
}
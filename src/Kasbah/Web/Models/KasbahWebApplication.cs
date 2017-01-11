using System;

namespace Kasbah.Web.Models
{
    public class KasbahWebApplication
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime Started { get; } = DateTime.UtcNow;
        public ulong RequestsTotal { get; set; } = 0;
    }
}
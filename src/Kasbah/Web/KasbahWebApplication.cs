using System;

namespace Kasbah.Web
{
    public class KasbahWebApplication
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime Started { get; } = DateTime.UtcNow;
    }
}
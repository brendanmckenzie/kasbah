using System;
using Microsoft.AspNetCore.Http;

namespace Kasbah.Web.Analytics.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetCurrentProfileId(this HttpContext context)
            => (Guid)context.Items["user:profile"];
    }
}

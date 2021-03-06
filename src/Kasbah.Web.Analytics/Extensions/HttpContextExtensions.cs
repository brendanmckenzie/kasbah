using System;
using Kasbah.Web.Analytics.Middleware;
using Microsoft.AspNetCore.Http;

namespace Kasbah.Web.Analytics.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetCurrentSessionId(this HttpContext context)
            => (Guid)context.Items[AnalyticsMiddleware.SessionKey];
    }
}

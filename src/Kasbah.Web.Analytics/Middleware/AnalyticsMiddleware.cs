using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.Analytics.Middleware
{
    public class AnalyticsMiddleware
    {
        public const string TrackingCookie = "kasbah_session";

        readonly RequestDelegate _next;
        readonly ILogger _log;

        public AnalyticsMiddleware(RequestDelegate next, ILogger<AnalyticsMiddleware> log)
        {
            _next = next;
            _log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            EnsureTrackingCookie(context);

            await _next.Invoke(context);
        }

        Guid GetTrackingCookieId(HttpContext context)
        {
            var query = context.Request.Query;
            if (query.TryGetValue(TrackingCookie, out var queryValue)
                && Guid.TryParse(queryValue, out var queryValueId))
            {
                return queryValueId;
            }

            var cookies = context.Request.Cookies;
            if (cookies.TryGetValue(TrackingCookie, out var cookieValue)
                && Guid.TryParse(cookieValue, out var cookieValueId))
            {
                return cookieValueId;
            }

            if (context.Items.ContainsKey(TrackingCookie))
            {
                return (Guid)context.Items[TrackingCookie];
            }

            return Guid.NewGuid();
        }

        void EnsureTrackingCookie(HttpContext context)
        {
            string IdToString(Guid id)
                => Convert.ToBase64String(id.ToByteArray());

            context.Items["user:profile"] = GetTrackingCookieId(context);
            context.Response.Cookies.Append(TrackingCookie, IdToString(GetTrackingCookieId(context)), new CookieOptions { Expires = DateTimeOffset.Now.AddYears(5) });
        }
    }
}

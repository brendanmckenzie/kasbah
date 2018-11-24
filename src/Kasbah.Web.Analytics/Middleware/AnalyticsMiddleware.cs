using System;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Analytics.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.Analytics.Middleware
{
    public class AnalyticsMiddleware
    {
        public const string TrackingCookie = "kasbah_session";
        public const string SessionKey = "kasbah:analytics:session";

        readonly RequestDelegate _next;
        readonly ILogger _log;
        readonly TrackingService _trackingService;

        public AnalyticsMiddleware(RequestDelegate next, ILogger<AnalyticsMiddleware> log, TrackingService trackingService)
        {
            _next = next;
            _log = log;
            _trackingService = trackingService;
        }

        public async Task Invoke(HttpContext context)
        {
            EnsureTrackingCookie(context);
            await TrackSessionActivityAsync(context);

            await _next.Invoke(context);
        }

        void EnsureTrackingCookie(HttpContext context)
        {
            var session = GetTrackingCookieId(context);

            string IdToString(Guid id)
                => Convert.ToBase64String(id.ToByteArray());

            context.Items[SessionKey] = session;

            context.Response.Cookies.Append(TrackingCookie, IdToString(session));
        }

        async Task TrackSessionActivityAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/.analytics"))
            {
                // don't track activity to activity tracking endpoints
                return;
            }

            var session = (Guid)context.Items[SessionKey];
            var kasbahWebContext = context.GetKasbahWebContext();

            context.Request.Headers.TryGetValue("User-Agent", out var userAgent);
            context.Request.Headers.TryGetValue("Referer", out var referrer);

            var data = new
            {
                url = $"{context.Request.Path}{context.Request.QueryString}",
                site = kasbahWebContext.Site?.Alias,
                node = kasbahWebContext.Node?.Id,
                version = kasbahWebContext.Node?.PublishedVersion,
                ip = RemoteIp(context),
                userAgent = userAgent.FirstOrDefault(),
                referrer = referrer.FirstOrDefault()
            };

            await _trackingService.TrackSessionActivityAsync(session, "request", data);
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
            if (cookies.TryGetValue(TrackingCookie, out var cookieValue))
            {
                return new Guid(Convert.FromBase64String(cookieValue));
            }

            if (context.Items.ContainsKey(TrackingCookie))
            {
                return (Guid)context.Items[TrackingCookie];
            }

            return Guid.NewGuid();
        }

        string RemoteIp(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var ip))
            {
                return ip.FirstOrDefault();
            }

            return context.Connection.RemoteIpAddress.ToString();
        }
    }
}

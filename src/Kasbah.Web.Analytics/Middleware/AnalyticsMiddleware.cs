using System;
using System.Threading.Tasks;
using Kasbah.Analytics;
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
        readonly SessionService _sessionService;
        readonly TrackingService _trackingService;

        public AnalyticsMiddleware(RequestDelegate next, ILogger<AnalyticsMiddleware> log, SessionService sessionService, TrackingService trackingService)
        {
            _next = next;
            _log = log;
            _sessionService = sessionService;
            _trackingService = trackingService;
        }

        public async Task Invoke(HttpContext context)
        {
            await EnsureTrackingCookieAsync(context);
            await TrackSessionActivityAsync(context);

            await _next.Invoke(context);
        }

        async Task EnsureTrackingCookieAsync(HttpContext context)
        {
            var session = await GetTrackingCookieIdAsync(context);

            string IdToString(Guid id)
                => Convert.ToBase64String(id.ToByteArray());

            context.Items[SessionKey] = await GetTrackingCookieIdAsync(context);

            context.Response.Cookies.Append(TrackingCookie, IdToString(session));
        }

        async Task TrackSessionActivityAsync(HttpContext context)
        {
            var session = (Guid)context.Items[SessionKey];
            var kasbahWebContext = context.GetKasbahWebContext();

            var userAgent = context.Request.Headers.SafeGet("User-Agent", string.Empty);

            var data = new
            {
                url = $"{context.Request.Path}{context.Request.QueryString}",
                site = kasbahWebContext.Site?.Alias,
                node = kasbahWebContext.Node?.Id,
                version = kasbahWebContext.Node?.PublishedVersion,
                ip = RemoteIp(context),
                userAgent
            };

            await _trackingService.TrackSessionActivityAsync(session, "request", data);
        }

        async Task<Guid> GetTrackingCookieIdAsync(HttpContext context)
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

            var id = Guid.NewGuid();

            await _sessionService.CreateSessionAsync(id);

            return id;
        }

        string RemoteIp(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var ip))
            {
                return ip;
            }

            return context.Connection.RemoteIpAddress.ToString();
        }
    }
}

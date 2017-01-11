using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.ContentDelivery.Middleware
{
    public class AnalyticsMiddleware
    {
        const string TrackingCookie = "__kastrk";

        readonly RequestDelegate _next;
        readonly ILogger _log;

        public AnalyticsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _log = loggerFactory.CreateLogger<AnalyticsMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            EnsureTrackingCookie(context);

            await CheckAndMergeProfilesAsync(context);

            await _next.Invoke(context);
        }

        string GetTrackingCookieId(HttpContext context)
        {
            var query = context.Request.Query;
            if (query.ContainsKey(TrackingCookie))
            {
                return query[TrackingCookie];
            }

            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        void EnsureTrackingCookie(HttpContext context)
        {
            var cookies = context.Request.Cookies;
            if (!cookies.ContainsKey(TrackingCookie))
            {
                context.Response.Cookies.Append(TrackingCookie, GetTrackingCookieId(context));
            }
        }

        async Task CheckAndMergeProfilesAsync(HttpContext context)
        {
            var query = context.Request.Query;
            var cookies = context.Request.Cookies;

            if (cookies.ContainsKey(TrackingCookie) && query.ContainsKey(TrackingCookie))
            {
                if (cookies[TrackingCookie] != query[TrackingCookie])
                {
                    // merge profiles
                    await Task.Delay(0);
                }
            }
        }
    }
}
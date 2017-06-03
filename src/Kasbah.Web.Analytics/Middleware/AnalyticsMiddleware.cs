using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.Analytics.Middleware
{
    public class AnalyticsMiddleware
    {
        public const string TrackingCookie = "__kastrk";

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

            var cookies = context.Request.Cookies;
            var cookieValue = default(string);
            if (cookies.TryGetValue(TrackingCookie, out cookieValue))
            {
                return cookieValue;
            }

            if (context.Items.ContainsKey(TrackingCookie))
            {
                return context.Items[TrackingCookie] as string;
            }

            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        void EnsureTrackingCookie(HttpContext context)
        {
            context.Items["user:profile"] = GetTrackingCookieId(context);
            context.Response.Cookies.Append(TrackingCookie, GetTrackingCookieId(context), new CookieOptions { Expires = DateTimeOffset.Now.AddYears(5) });
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

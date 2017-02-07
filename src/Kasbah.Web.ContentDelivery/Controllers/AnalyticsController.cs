using System;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Analytics.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentDelivery.Controllers
{
    [Route("analytics")]
    public class AnalyticsController : Controller
    {
        readonly AnalyticsService _analyticsService;

        public AnalyticsController(AnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [Route("track"), HttpPost]
        public async Task TrackEvent([FromBody] AnalyticsEvent ev)
        {
            var profile = ControllerContext.HttpContext.Items["user:profile"] as string;
            if (!string.IsNullOrEmpty(profile))
            {
                ev.Profile = new Guid(Convert.FromBase64String(profile));
            }

            await _analyticsService.TrackEventAsync(ev);
        }

        [Route("tracker.js"), HttpGet]
        public FileResult Tracker()
        {
            var resourceName = "Kasbah.Web.ContentDelivery.Resources.AnalyticsTracker.js";
            var assembly = typeof(AnalyticsController).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(resourceName);

            return new FileStreamResult(stream, "application/javascript");
        }
    }
}

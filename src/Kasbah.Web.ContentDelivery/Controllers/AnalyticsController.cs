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
            await _analyticsService.TrackEvent(ev);
        }

        [Route("init"), HttpPost]
        public async Task<AnalyticsInitResponse> Init([FromBody] AnalyticsInitRequest request)
        {
            if (request?.Persona.HasValue == true)
            {
                return new AnalyticsInitResponse
                {
                    Persona = request.Persona.Value
                };
            }
            else
            {
                return new AnalyticsInitResponse
                {
                    Persona = await _analyticsService.CreatePersonaAsync()
                };
            }
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

    public class AnalyticsInitRequest
    {
        public Guid? Persona { get; set; }
    }

    public class AnalyticsInitResponse
    {
        public Guid Persona { get; set; }
        public Guid Session { get; set; }
    }
}

using System;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Analytics.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentDelivery.Controllers
{
    [Route("analytics")]
    public class AnalyticsController
    {
        readonly AnalyticsService _analyticsService;

        public AnalyticsController(AnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [Route("track")]
        public async Task TrackEvent(AnalyticsEvent ev)
        {
            await _analyticsService.TrackEvent(ev);
        }

        [Route("init")]
        public async Task<AnalyticsInitResponse> Init([FromBody] AnalyticsInitRequest request)
        {
            if (request.Persona.HasValue)
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

        [Route("tracker.js")]
        public FileResult Tracker()
        {
            var resourceName = "Kasbah.Web.ContentDelivery.Resources.AnalyticsTracker.js";
            var assembly = typeof(AnalyticsController).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(resourceName);

            return new FileStreamResult(stream, "text/javascript");
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
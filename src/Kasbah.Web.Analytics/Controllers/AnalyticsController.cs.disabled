using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Web.ContentDelivery.Extensions;
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

        [Route("event"), HttpPost]
        public async Task TrackEventAsync([FromBody] TrackEventRequest ev)
            => await _analyticsService.TrackEventAsync(ControllerContext.HttpContext.GetCurrentProfileId(), ev.Type, ev.Source, ev.Data);

        [Route("bias"), HttpPost]
        public async Task TriggerBiasAsync([FromBody] TriggerBiasRequest request)
            => await _analyticsService.TriggerBiasAsync(ControllerContext.HttpContext.GetCurrentProfileId(), request.Bias, request.Weight);

        [Route("attributes"), HttpPost]
        public async Task SetAttributesRequest([FromBody] SetAttributesRequest request)
            => await _analyticsService.SetAttributesAsync(ControllerContext.HttpContext.GetCurrentProfileId(), request.Attributes);

        [Route("tracker.js"), HttpGet]
        public FileResult Tracker()
        {
            var resourceName = "Kasbah.Web.ContentDelivery.Resources.AnalyticsTracker.js";
            var assembly = typeof(AnalyticsController).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(resourceName);

            return new FileStreamResult(stream, "application/javascript");
        }
    }

    public class TrackEventRequest
    {
        public string Type { get; set; }
        public string Source { get; set; }
        public IDictionary<string, string> Data { get; set; }
    }

    public class TriggerBiasRequest
    {
        public string Bias { get; set; }
        public long Weight { get; set; }
    }

    public class SetAttributesRequest
    {
        public IDictionary<string, string> Attributes { get; set; }
    }
}

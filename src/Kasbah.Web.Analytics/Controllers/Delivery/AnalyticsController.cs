using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Analytics.Services;
using Kasbah.Web.Analytics.Extensions;
using Kasbah.Web.Analytics.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.Analytics.Controllers.Delivery
{
    [Route(".analytics/tracking")]
    public class AnalyticsController : Controller
    {
        readonly TrackingService _trackingService;

        public AnalyticsController(TrackingService trackingService)
        {
            _trackingService = trackingService;
        }

        [HttpPost]
        [Route("activity")]
        public async Task TrackSessionActivityAsync([FromBody] TrackSessionActivityRequest ev)
            => await _trackingService.TrackSessionActivityAsync(HttpContext.GetCurrentSessionId(), ev.Type, ev.Data);

        [HttpPost]
        [Route("event")]
        public async Task TrackEventAsync([FromBody] TrackEventRequest ev)
            => await _trackingService.TrackEventAsync(HttpContext.GetCurrentSessionId(), ev.Type, ev.Source, ev.Data);

        [HttpPost]
        [Route("campaign")]
        public async Task TrackCampaignAsync([FromBody] TriggerCampaignRequest ev)
            => await _trackingService.TrackCampaignAsync(HttpContext.GetCurrentSessionId(), ev.Campaign);

        [HttpPost]
        [Route("attributes")]
        public async Task SetAttributes([FromBody] SetAttributesRequest ev)
        {
            foreach (var key in ev.Attributes.Keys)
            {
                await _trackingService.CreateAttributeAsync(HttpContext.GetCurrentSessionId(), key, ev.Attributes[key]);
            }
        }
    }
}

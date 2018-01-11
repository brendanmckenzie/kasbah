using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Web.Analytics.Extensions;
using Kasbah.Web.Analytics.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentDelivery.Controllers
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
        [Route("event")]
        public async Task TrackEventAsync([FromBody] TrackEventRequest ev)
            => await _trackingService.TrackEventAsync(HttpContext.GetCurrentProfileId(), ev.Type, ev.Source, ev.Data);

        [HttpPost]
        [Route("campaign")]
        public async Task TrackCampaignAsync([FromBody] TriggerCampaignRequest ev)
            => await _trackingService.TrackCampaignAsync(HttpContext.GetCurrentProfileId(), ev.Campaign);

        [HttpPost]
        [Route("attributes")]
        public async Task SetAttributes([FromBody] SetAttributesRequest ev)
        {
            foreach (var key in ev.Attributes.Keys)
            {
                await _trackingService.CreateAttributeAsync(HttpContext.GetCurrentProfileId(), key, ev.Attributes[key]);
            }
        }
    }
}

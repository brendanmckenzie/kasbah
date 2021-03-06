using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Analytics.Models;
using Kasbah.Analytics.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.Analytics.Controllers.Management
{
    [Route("analytics/reporting")]
    public class ReportingController : Controller
    {
        readonly ReportingService _reportingService;

        public ReportingController(ReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        [Route("session-activity")]
        [HttpGet]
        public async Task<IEnumerable<ReportingData>> ListSessionActivityReportingAsync([FromQuery] string type, [FromQuery] string interval, [FromQuery] DateTime start, [FromQuery] DateTime end)
            => await _reportingService.ListSessionActivityReportingAsync(type, interval, start, end);

        [Route("session")]
        [HttpGet]
        public async Task<IEnumerable<ReportingData>> ListSessionReportingAsync([FromQuery] string interval, [FromQuery] DateTime start, [FromQuery] DateTime end)
            => await _reportingService.ListSessionReportingAsync(interval, start, end);

        [Route("sessions")]
        [HttpGet]
        public async Task<IEnumerable<Session>> ListSessionsAsync([FromQuery] int skip, [FromQuery] int take)
            => await _reportingService.ListSessionsAsync(skip, take);

        [Route("sessions/{session}")]
        [HttpGet]
        public async Task<IEnumerable<SessionActivity>> ListSessionActivityAsync([FromRoute] Guid session, [FromQuery] int skip, [FromQuery] int take, [FromQuery] string type = null)
            => await _reportingService.ListSessionActivityAsync(session, skip, take, type);
    }
}

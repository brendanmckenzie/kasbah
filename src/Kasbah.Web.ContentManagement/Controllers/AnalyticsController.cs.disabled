using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Analytics;
using Kasbah.Analytics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentManagement.Controllers
{
    [Authorize]
    [Route("analytics")]
    public class AnalyticsController : Controller
    {
        readonly AnalyticsService _analyticsService;

        public AnalyticsController(AnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [Route("profiles/list"), HttpGet]
        public async Task<IEnumerable<ProfileSummary>> ListProfilesAsync()
            => await _analyticsService.ListProfilesAsync();

        [Route("profiles/{id}"), HttpGet]
        public async Task<Profile> GetProfileAsync(Guid id)
            => await _analyticsService.GetProfileAsync(id);
    }
}

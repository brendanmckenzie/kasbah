using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Web.ContentDelivery.Extensions;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.ContentDelivery.Middleware
{
    public class KasbahWebContextInitialisationMiddleware
    {
        readonly RequestDelegate _next;
        readonly ILogger _log;
        readonly ContentService _contentService;
        readonly SiteRegistry _siteRegistry;
        readonly TypeRegistry _typeRegistry;
        readonly KasbahWebApplication _kasbahWebApplication;
        readonly TypeMapper _typeMapper;

        public KasbahWebContextInitialisationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, ContentService contentService, SiteRegistry siteRegistry, TypeRegistry typeRegistry, KasbahWebApplication kasbahWebApplication, TypeMapper typeMapper)
        {
            _next = next;
            _log = loggerFactory.CreateLogger<KasbahWebContextInitialisationMiddleware>();
            _contentService = contentService;
            _siteRegistry = siteRegistry;
            _typeRegistry = typeRegistry;
            _kasbahWebApplication = kasbahWebApplication;
            _typeMapper = typeMapper;
        }

        public async Task Invoke(HttpContext context)
        {
            var kasbahWebContext = new KasbahWebContext
            {
                WebApplication = _kasbahWebApplication,
                HttpContext = context,
                ContentService = _contentService,
                TypeRegistry = _typeRegistry,
                TypeMapper = _typeMapper,
                SiteRegistry = _siteRegistry
            };

            context.Items["kasbahWebContext"] = kasbahWebContext;

            await _next.Invoke(context);
        }
    }
}

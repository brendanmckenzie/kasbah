using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;

namespace Kasbah.Web.Middleware.Delivery
{
    public class KasbahProfilingMiddleware
    {
        readonly RequestDelegate _next;
        readonly ILogger _log;

        public KasbahProfilingMiddleware(RequestDelegate next, ILogger<KasbahProfilingMiddleware> log)
        {
            _next = next;
            _log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            var profiler = MiniProfiler.StartNew($"Request - {context.TraceIdentifier} ('{context.Request.Path}')");
            try
            {
                await _next.Invoke(context);
            }
            finally
            {
                profiler.Stop();

                _log.LogInformation(profiler.RenderPlainText());
            }
        }
    }
}

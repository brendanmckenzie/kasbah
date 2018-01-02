using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Prerendering;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.Middleware.Delivery
{
    public class KasbahRouterMiddleware
    {
        readonly RequestDelegate _next;
        readonly ILogger _log;
        readonly ISpaPrerenderer _prerenderer;

        public KasbahRouterMiddleware(RequestDelegate next, ILogger<KasbahRouterMiddleware> log, ISpaPrerenderer prerenderer)
        {
            _next = next;
            _log = log;
            _prerenderer = prerenderer;
        }

        public async Task Invoke(HttpContext context)
        {
            var kasbahWebContext = context.GetKasbahWebContext();

            if (kasbahWebContext.Site != null)
            {
                var node = kasbahWebContext.Node;

                if (node != null && node.PublishedVersion.HasValue)
                {
                    var model = new
                    {
                        Node = node,
                        Site = kasbahWebContext.Site,
                        SiteNode = kasbahWebContext.SiteNode
                    };

                    var result = await _prerenderer.RenderToString("wwwroot/dist/kasbah-server", customDataParameter: model);

                    if (!string.IsNullOrEmpty(result.RedirectUrl))
                    {
                        context.Response.Redirect(result.RedirectUrl, false);
                    }
                    else
                    {
                        await context.Response.WriteHtmlAsync($"<!DOCTYPE html>{result.Html}");
                    }

                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}

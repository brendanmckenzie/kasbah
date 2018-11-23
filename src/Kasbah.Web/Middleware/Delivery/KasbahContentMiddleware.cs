using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Prerendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kasbah.Web.Middleware.Delivery
{
    public class KasbahContentMiddleware
    {
        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        readonly RequestDelegate _next;
        readonly ILogger _log;
        readonly ISpaPrerenderer _prerenderer;
        readonly ComponentRegistry _componentRegistry;

        public KasbahContentMiddleware(RequestDelegate next, ILogger<KasbahRouterMiddleware> log, ISpaPrerenderer prerenderer, ComponentRegistry componentRegistry)
        {
            _next = next;
            _log = log;
            _prerenderer = prerenderer;
            _componentRegistry = componentRegistry;
        }

        public async Task Invoke(HttpContext context)
        {
            var model = context.Items["kasbah:model"];
            if (model == null)
            {
                await _next.Invoke(context);
            }
            else
            {
                if (context.Request.Query.TryGetValue("ct", out var qs) && qs.Contains("json"))
                {
                    await ProcessJsonAsync(model, context.Response);
                }
                else
                {
                    await ProcessDocumentAsync(model, context.Response);
                }
            }
        }

        async Task ProcessJsonAsync(object model, HttpResponse response)
        {
            var json = JsonConvert.SerializeObject(model, JsonSettings);

            response.StatusCode = 200;

            await response.WriteJsonAsync(json);
        }

        async Task ProcessDocumentAsync(object model, HttpResponse response)
        {
            var result = await _prerenderer.RenderToString("wwwroot/dist/kasbah-server", customDataParameter: model);

            if (!string.IsNullOrEmpty(result.RedirectUrl))
            {
                response.Redirect(result.RedirectUrl, false);
            }
            else
            {
                response.StatusCode = 200;

                await response.WriteHtmlAsync($"<!DOCTYPE html>{result.Html}");
            }
        }
    }
}

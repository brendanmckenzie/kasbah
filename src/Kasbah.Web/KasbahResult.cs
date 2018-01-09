using System;
using System.Threading.Tasks;
using Kasbah.Web.Middleware.Delivery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Prerendering;

namespace Kasbah.Web
{
    public class KasbahResult : ActionResult
    {
        readonly object _model;

        public KasbahResult(object model)
        {
            _model = model;
        }

        public override void ExecuteResult(ActionContext context)
        {
            ExecuteResultAsync(context).Wait();
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.HttpContext.Request.ContentType?.Equals("application/json") == true)
            {
                await new JsonResult(_model, KasbahContentMiddleware.JsonSettings).ExecuteResultAsync(context);
            }
            else
            {
                var prerenderer = context.HttpContext.RequestServices.GetService(typeof(ISpaPrerenderer)) as ISpaPrerenderer;
                var result = await prerenderer.RenderToString("wwwroot/dist/kasbah-server", customDataParameter: _model);

                if (!string.IsNullOrEmpty(result.RedirectUrl))
                {
                    await new RedirectResult(result.RedirectUrl).ExecuteResultAsync(context);
                }
                else
                {
                    var res = new ContentResult
                    {
                        Content = $"<!DOCTYPE html>{result.Html}",
                        ContentType = "text/html"
                    };

                    await res.ExecuteResultAsync(context);
                }
            }
        }
    }
}

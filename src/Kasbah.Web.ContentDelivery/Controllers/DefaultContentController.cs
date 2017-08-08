using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentDelivery.Controllers
{
    public class DefaultContentController : Controller
    {
        public ActionResult RenderContent()
        {
            var content = RouteData.Values["content"];
            var area = RouteData.Values["area"] as string;
            var viewName = RouteData.Values["view"] as string;
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = nameof(RenderContent);
            }

            return View(viewName, content);
        }
    }
}

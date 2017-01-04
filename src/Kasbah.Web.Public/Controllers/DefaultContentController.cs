using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.Public.Controllers
{
    public class DefaultContentController : Controller
    {
        #region Public Methods

        public ActionResult RenderContent()
        {
            var content = RouteData.Values["content"];
            var viewName = (RouteData.Values["view"] as string);
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = nameof(RenderContent);
            }

            return View(viewName, content);
        }

        #endregion
    }
}
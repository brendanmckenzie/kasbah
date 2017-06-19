using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentDelivery.ViewComponents
{
    public class PlaceholderViewComponent : ViewComponent
    {
        readonly ContentService _contentService;
        public PlaceholderViewComponent(ContentService contentService)
        {
            _contentService = contentService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var kasbahWebContext = RouteData.Values["kasbahWebContext"] as KasbahWebContext;

            if (kasbahWebContext == null)
            {
                // _log.LogWarning($"An attempt has been made to render modules on a page that is not managed by the content tree. View: '{htmlHelper.ViewContext.View.Path}' Section: {section}");

                return null;
            }

            if (kasbahWebContext.Node == null)
            {
                // _log.LogWarning($"An attempt has been made to render modules on a page that is not managed by the content tree. View: '{htmlHelper.ViewContext.View.Path}' Section: {section}");

                return null;
            }

            var node = kasbahWebContext.Node;
            var data = await _contentService.GetRawDataAsync(node.Id, node.PublishedVersion);
            var content = await kasbahWebContext.TypeMapper.MapTypeAsync(data, node.Type, node, node.PublishedVersion);

            if (content is Presentable)
            {
                var components = (content as Presentable).Components[section];

                var ret = await Task.WhenAll(components.AsParallel().Select(async ent => await viewComponentHelper.InvokeAsync(ent.Control, ent.DataSource)));

                return htmlHelper.Raw(string.Join(Environment.NewLine, ret.Where(ent => ent != null).Select(ContentToString)));
            }
            else
            {
                // log warning
            }
        }
    }
}

using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Kasbah.Web.ContentDelivery.Extensions
{
    public static class HtmlExtensions
    {
        #region Public Methods

        /// <summary>
        /// Renders all modules that belong to <paramref name="section"/>.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="section">The section to render.</param>
        /// <param name="viewComponentHelper">The view component helper.</param>
        /// <returns>The rendered HTML content from the matching modules.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<IHtmlContent> PlaceholderAsync(this IHtmlHelper htmlHelper, string section, IViewComponentHelper viewComponentHelper)
        {
            var kasbahWebContext = htmlHelper.ViewContext.RouteData.Values["kasbahWebContext"] as KasbahWebContext;

            if (kasbahWebContext == null)
            {
                // _log.LogWarning($"An attempt has been made to render modules on a page that is not managed by the content tree. View: '{htmlHelper.ViewContext.View.Path}' Section: {section}");

                return null;
            }

            var contentService = htmlHelper.ViewContext.RouteData.Values["contentService"] as ContentService;

            if (kasbahWebContext.Node == null)
            {
                // _log.LogWarning($"An attempt has been made to render modules on a page that is not managed by the content tree. View: '{htmlHelper.ViewContext.View.Path}' Section: {section}");

                return null;
            }

            var node = kasbahWebContext.Node;
            var data = await contentService.GetRawDataAsync(node.Id, node.PublishedVersion);
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

            return new StringHtmlContent(string.Empty);
        }

        #endregion

        #region Private Methods

        static string ContentToString(IHtmlContent content)
        {
            if (content == null) { return null; }

            using (var writer = new System.IO.StringWriter())
            {
                content.WriteTo(writer, HtmlEncoder.Default);

                return writer.ToString();
            }
        }

        #endregion
    }
}

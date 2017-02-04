using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Kasbah.Content;
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
        public static async Task<IHtmlContent> ModulesAsync(this IHtmlHelper htmlHelper, string section, IViewComponentHelper viewComponentHelper = null)
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

            var modulesRoot = await contentService.GetChildByAliasAsync(kasbahWebContext.Node.Id, "modules");
            if (modulesRoot != null)
            {
                var moduleRoot = await contentService.GetChildByAliasAsync(modulesRoot.Id, section);
                if (moduleRoot != null)
                {
                    var moduleNodes = (await contentService.GetChildrenAsync(moduleRoot.Id)).OrderBy(ent => ent.Alias);
                    var ret = new List<IHtmlContent>();
                    foreach (var moduleNode in moduleNodes)
                    {
                        if (!moduleNode.PublishedVersion.HasValue)
                        {
                            continue;
                        }
                        var typeDefinition = kasbahWebContext.TypeRegistry.GetType(moduleNode.Type);
                        var moduleNodeData = await kasbahWebContext.ContentService.GetRawDataAsync(moduleNode.Id, moduleNode.PublishedVersion);
                        var module = kasbahWebContext.TypeMapper.MapTypeAsync(moduleNodeData, moduleNode.Type, moduleNode);
                        var view = typeDefinition.Options.SafeGet("view");
                        var viewComponent = typeDefinition.Options.SafeGet("viewComponent");
                        if (viewComponent != null)
                        {
                            if (viewComponentHelper == null) { throw new ArgumentNullException(nameof(viewComponentHelper)); }

                            ret.Add(await viewComponentHelper.InvokeAsync(viewComponent as string, module));
                        }
                        else if (view != null)
                        {
                            ret.Add(await htmlHelper.PartialAsync(view as string, module, null));
                        }
                    }

                    if (ret.Any())
                    {
                        return htmlHelper.Raw(string.Join(Environment.NewLine, ret.Select(ContentToString)));
                    }
                }
                else
                {
                    // _log.LogWarning($"An attempt has been made to render modules where no modules exist for this section.  View: '{htmlHelper.ViewContext.View.Path}' Section: {section}");
                }
            }
            else
            {
                // _log.LogWarning($"An attempt has been made to render modules where no modules exist.  View: '{htmlHelper.ViewContext.View.Path}' Section: {section}");
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

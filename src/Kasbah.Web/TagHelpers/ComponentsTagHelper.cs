using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.TagHelpers
{
    [HtmlTargetElement("components")]
    public class ComponentsTagHelper : TagHelper
    {
        readonly ILogger _log;
        readonly DefaultViewComponentHelper _viewComponentHelper;
        readonly ContentService _contentService;
        readonly ComponentRegistry _componentRegistry;

        public ComponentsTagHelper(ILogger<ComponentsTagHelper> log, IViewComponentHelper viewComponentHelper, ContentService contentService, ComponentRegistry componentRegistry)
        {
            _log = log;
            _viewComponentHelper = viewComponentHelper as DefaultViewComponentHelper;
            _contentService = contentService;
            _componentRegistry = componentRegistry;
        }

        [HtmlAttributeName("area")]
        public string Area { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var kasbahWebContext = ViewContext.HttpContext.GetKasbahWebContext();

            if (kasbahWebContext == null)
            {
                _log.LogWarning($"An attempt has been made to render modules on a page that is not managed by the content tree. View: '{ViewContext.View.Path}' Area: {Area}");

                return;
            }

            if (kasbahWebContext.Node == null)
            {
                _log.LogWarning($"An attempt has been made to render modules on a page that is not managed by the content tree. View: '{ViewContext.View.Path}' Area: {Area}");

                return;
            }

            var node = kasbahWebContext.Node;
            if (typeof(IPresentable).GetTypeInfo().IsAssignableFrom(Type.GetType(kasbahWebContext.TypeRegistry.GetType(node.Type).Alias)))
            {
                var data = await _contentService.GetRawDataAsync(node.Id, node.PublishedVersion);
                var content = await kasbahWebContext.TypeMapper.MapTypeAsync(data, node.Type, node, node.PublishedVersion);

                if (content is IPresentable presentable)
                {
                    if (presentable.Components?.ContainsKey(Area) == true)
                    {
                        _viewComponentHelper.Contextualize(ViewContext);

                        var ret = await Task.WhenAll(presentable.Components[Area].AsParallel().AsOrdered().Select(async ent =>
                        {
                            var component = _componentRegistry.GetByAlias(ent.Control);

                            if (ent.Properties != null && component.Properties != null)
                            {
                                var properties = await kasbahWebContext.TypeMapper.MapTypeAsync(ent.Properties, component.Properties.Alias);

                                return await _viewComponentHelper.InvokeAsync(component.Control, properties);
                            }
                            else
                            {
                                return await _viewComponentHelper.InvokeAsync(component.Control, content);
                            }
                        }));

                        output.TagName = string.Empty;

                        foreach (var block in ret)
                        {
                            output.Content.AppendHtml(block);
                        }
                    }
                }
            }
            else
            {
                _log.LogWarning($"An attempt has been made to render modules on a page that does not inherit from Presentable. View: '{ViewContext.View.Path}' Area: {Area}");
            }
        }
    }
}

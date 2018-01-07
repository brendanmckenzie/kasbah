using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Content.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.Controllers.Delivery
{
    [Route(".kasbah")]
    public class ContentController : Controller
    {
        readonly ContentService _contentService;
        readonly TypeRegistry _typeRegistry;
        readonly TypeMapper _typeMapper;

        public ContentController(ContentService contentService, TypeRegistry typeRegistry, TypeMapper typeMapper)
        {
            _contentService = contentService;
            _typeRegistry = typeRegistry;
            _typeMapper = typeMapper;
        }

        [Route("content")]
        [HttpGet]
        public async Task<object> GetContent([FromQuery] Guid id, [FromQuery] int? version)
        {
            var node = await _contentService.GetNodeAsync(id);

            var type = _typeRegistry.GetType(node.Type);

            var data = await _contentService.GetRawDataAsync(node.Id, version ?? node.PublishedVersion);
            var content = await _typeMapper.MapTypeAsync(data, node.Type, node);

            return content;
        }

        [Route("children")]
        [HttpGet]
        public async Task<IEnumerable<Node>> ListChildren([FromQuery] Guid id)
        {
            var node = await _contentService.GetNodeAsync(id);

            return node.Children.Value;
        }
    }
}

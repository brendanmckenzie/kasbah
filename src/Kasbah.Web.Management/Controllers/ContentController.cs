using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Web.ContentManagement.ViewModels;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Kasbah.Web.Management.Controllers
{
    [Authorize]
    [Route("content")]
    public class ContentController : Controller
    {
        readonly ContentService _contentService;
        readonly TypeRegistry _typeRegistry;
        readonly ComponentRegistry _componentRegistry;
        readonly SiteRegistry _siteRegistry;

        public ContentController(ContentService contentService, TypeRegistry typeRegistry, ComponentRegistry componentRegistry, SiteRegistry siteRegistry)
        {
            _contentService = contentService;
            _typeRegistry = typeRegistry;
            _componentRegistry = componentRegistry;
            _siteRegistry = siteRegistry;
        }

        [Route("{id}/edit")]
        [HttpGet]
        public async Task<NodeDataForEditing> GetNodeDataForEditing(Guid id)
        {
            var ret = await _contentService.GetNodeDataForEditingAsync(id);

            var site = _siteRegistry.GetSiteByNode(ret.Node);

            ret.Data["_url"] = site?.ItemUrl(ret.Node, true).ToString();

            return ret;
        }

        [Route("{id}")]
        [HttpPost]
        public async Task<NodeDataForEditing> UpdateNodeData(Guid id, [FromBody]IDictionary<string, object> data, [FromQuery] bool publish)
        {
            foreach (var kvp in data.Where(ent => ent.Value is JObject).ToList())
            {
                data[kvp.Key] = (kvp.Value as JObject).ToObject<IDictionary<string, object>>();
            }

            foreach (var kvp in data.Where(ent => ent.Value is JArray).ToList())
            {
                data[kvp.Key] = (kvp.Value as JArray).ToArray();
            }

            await _contentService.UpdateDataAsync(id, data, publish);

            return await GetNodeDataForEditing(id);
        }

        [Route("tree")]
        [HttpGet]
        public async Task<IEnumerable<Node>> DescribeTree()
            => await _contentService.DescribeTreeAsync();

        [Route("types")]
        [HttpGet]
        public async Task<IEnumerable<TypeDefinition>> ListTypes()
            => await Task.FromResult(_typeRegistry.ListTypes());

        [Route("components")]
        [HttpGet]
        public async Task<IEnumerable<ComponentDefinition>> ListComponents()
            => await Task.FromResult(_componentRegistry.ListComponents());

        [Route("node")]
        [HttpPost]
        public async Task<Guid> CreateNode([FromBody] CreateNodeRequest request)
            => await _contentService.CreateNodeAsync(request.Parent, request.Alias, request.Type, request.DisplayName);

        [Route("node/{id}")]
        [HttpGet]
        public async Task<Node> GetNode(Guid id)
            => await _contentService.GetNodeAsync(id);

        [Route("nodes/by-type")]
        [HttpPost]
        public async Task<IEnumerable<Node>> GetNodesByType([FromBody] GetNodesByTypeRequest request)
            => await _contentService.GetNodesByType(request.Type, request.Inherit);

        [Route("nodes/recent")]
        [HttpGet]
        public async Task<IEnumerable<Node>> GetRecentlyModified([FromQuery] int take)
            => await _contentService.GetRecentlyModified(take);

        [Route("node/{id}")]
        [HttpDelete]
        public async Task<EmptyResponse> DeleteNodeAsync(Guid id)
        {
            await _contentService.DeleteNodeAsync(id);

            return new EmptyResponse { };
        }

        [Route("node/{id}/move")]
        [HttpPut]
        public async Task<EmptyResponse> MoveNodeAsync(Guid id, [FromQuery] Guid? parent)
        {
            await _contentService.MoveNodeAsync(id, parent);

            return new EmptyResponse { };
        }

        [Route("node")]
        [HttpPut]
        public async Task<Node> PutNodeAsync([FromBody] Node node)
            => await _contentService.PutNodeAsync(node);
    }
}

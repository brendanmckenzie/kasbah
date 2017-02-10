using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Content.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Kasbah.Web.ContentManagement.Controllers
{
    [Authorize]
    [Route("content")]
    public class ContentController
    {
        readonly ContentService _contentService;
        readonly TypeRegistry _typeRegistry;
        public ContentController(ContentService contentService, TypeRegistry typeRegistry)
        {
            _contentService = contentService;
            _typeRegistry = typeRegistry;
        }

        [Route("{id}/edit"), HttpGet]
        public async Task<NodeDataForEditing> GetNodeDataForEditing(Guid id)
            => await _contentService.GetNodeDataForEditingAsync(id);

        [Route("{id}"), HttpPost]
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

        [Route("tree"), HttpGet]
        public async Task<IEnumerable<Node>> DescribeTree()
            => await _contentService.DescribeTreeAsync();

        [Route("types"), HttpGet]
        public async Task<IEnumerable<TypeDefinition>> ListTypes()
            => await Task.FromResult(_typeRegistry.ListTypes());

        [Route("node"), HttpPost]
        public async Task<Guid> CreateNode([FromBody] CreateNodeRequest request)
            => await _contentService.CreateNodeAsync(request.Parent, request.Alias, request.Type, request.DisplayName);

        [Route("node/{id}"), HttpGet]
        public async Task<Node> GetNode(Guid id)
            => await _contentService.GetNodeAsync(id);

        [Route("nodes/by-type"), HttpPost]
        public async Task<IEnumerable<Node>> GetNodesByType([FromBody] GetNodesByTypeRequest request)
            => await _contentService.GetNodesByType(request.Type, request.Inherit);

        [Route("nodes/recent"), HttpGet]
        public async Task<IEnumerable<Node>> GetRecentlyModified([FromQuery] int take)
            => await _contentService.GetRecentlyModified(take);

        [Route("node/{id}"), HttpDelete]
        public async Task<long> DeleteNodeAsync(Guid id)
            => await _contentService.DeleteNodeAsync(id);
    }

    public class CreateNodeRequest
    {
        public Guid? Parent { get; set; }
        public string Alias { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
    }

    public class GetNodesByTypeRequest
    {
        public string Type { get; set; }
        public bool Inherit { get; set; }
    }
}

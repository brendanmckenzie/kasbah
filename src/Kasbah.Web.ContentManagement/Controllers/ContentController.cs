using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Content.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentManagement.Controllers
{
    // [Authorize]
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

        [Route("{id}/publish/{version}"), HttpPost]
        public async Task<NodeDataForEditing> PublishNodeVersion(Guid id, int? version)
        {
            await _contentService.PublishNodeVersionAsync(id, version);

            return await GetNodeDataForEditing(id);
        }

        [Route("{id}"), HttpPost]
        public async Task<NodeDataForEditing> UpdateNodeData(Guid id, [FromBody]IDictionary<string, object> data)
        {
            await _contentService.UpdateDataAsync(id, data);

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
    }

    public class CreateNodeRequest
    {
        public Guid? Parent { get; set; }
        public string Alias { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
    }
}
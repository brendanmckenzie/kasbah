using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Content.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.Admin.Controllers
{
    // [Authorize]
    [Route("content")]
    public class ContentController
    {
        readonly ContentService _contentService;
        public ContentController(ContentService contentService)
        {
            _contentService = contentService;
        }

        [Route("{id}/edit"), HttpGet]
        public async Task<NodeDataForEditing> GetNodeDataForEditing(Guid id)
            => await _contentService.GetNodeDataForEditingAsync(id);

        [Route("{id}/publish/{version}"), HttpPost]
        public async Task PublishNodeVersion(Guid id, int? version)
            => await _contentService.PublishNodeVersionAsync(id, version);

        [Route("{id}"), HttpPost]
        public async Task UpdateNodeData(Guid id, [FromBody]IDictionary<string, object> data)
            => await _contentService.UpdateDataAsync(id, data);

        [Route("tree"), HttpGet]
        public async Task<IEnumerable<Node>> DescribeTree()
            => await _contentService.DescribeTreeAsync();

        [Route("node"), HttpPost]
        public async Task<Guid> CreateNode([FromBody] CreateNodeRequest request)
            => await _contentService.CreateNodeAsync(request.Parent, request.Alias, request.Type);
    }

    public class CreateNodeRequest
    {
        public Guid? Parent { get; set; }
        public string Alias { get; set; }
        public string Type { get; set; }
    }
}
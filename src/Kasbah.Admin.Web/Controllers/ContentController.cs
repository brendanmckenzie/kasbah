using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Content;
using Kasbah.Content.Models;

namespace Kasbah.Admin.Web.Controllers
{
    public class ContentController
    {
        readonly ContentService _contentService;
        public ContentController(ContentService contentService)
        {
            _contentService = contentService;
        }

        public async Task<IEnumerable<Node>> ListChildren(Guid? parent)
            => await _contentService.ListChildrenAsync(parent);
    }
}
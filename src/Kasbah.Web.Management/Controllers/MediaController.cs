using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Media;
using Kasbah.Media.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.Management.Controllers
{
    [Authorize]
    [Route("media")]
    public class MediaController : Controller
    {
        readonly MediaService _mediaService;

        public MediaController(MediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [Route("list")]
        [HttpGet]
        public async Task<IEnumerable<MediaItem>> List()
            => await _mediaService.ListMediaAsync();

        [Route("upload")]
        [HttpPost]
        public async Task<IEnumerable<Guid>> Upload(IEnumerable<IFormFile> files)
            => await Task.WhenAll(files.Concat(Request.Form.Files).Select(async ent => await _mediaService.CreateMediaAsync(ent.OpenReadStream(), ent.FileName, ent.ContentType)));

        [Route("{id}")]
        [HttpDelete]
        public async Task<bool> DeleteAsync(Guid id)
        {
            await _mediaService.DeleteMediaItemAsync(id);

            return true;
        }

        [Route("{id}/meta")]
        [HttpGet]
        public async Task<MediaItem> GetMediaMeta(Guid id)
            => await _mediaService.GetMediaItemAsync(id);

        [Route("")]
        [HttpPut]
        public async Task<MediaItem> PutMediaAsync([FromBody] MediaItem item)
            => await _mediaService.PutMediaAsync(item);
    }
}

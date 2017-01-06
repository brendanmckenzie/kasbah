using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Media;
using Kasbah.Media.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.Admin.Controllers
{
    [Route("media")]
    public class MediaController : Controller
    {
        readonly MediaService _mediaService;

        public MediaController(MediaService mediaService)
        {
            _mediaService = mediaService;
        }
        [Route("list"), HttpGet]
        public async Task<IEnumerable<MediaItem>> List()
            => await _mediaService.ListMediaAsync();

        [Route("upload"), HttpPost]
        public async Task<IEnumerable<Guid>> Upload(IEnumerable<IFormFile> files)
            => await Task.WhenAll(files.Concat(Request.Form.Files).Select(async ent => await _mediaService.PutMediaAsync(ent.OpenReadStream(), ent.FileName, ent.ContentType)));

        [Route("{id}"), HttpGet]
        public async Task<FileResult> GetMedia(Guid id, [FromQuery] GetMediaRequest request)
        {
            var item = await _mediaService.GetMediaItemAsync(id);
            var stream = await _mediaService.GetMediaStreamAsync(id);

            if (item.ContentType.StartsWith("image/"))
            {
                // TODO: process image based on `request` as necessary
            }

            return new FileStreamResult(stream, item.ContentType);
        }
    }

    public class GetMediaRequest
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}
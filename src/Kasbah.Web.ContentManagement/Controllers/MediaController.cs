using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kasbah.Media;
using Kasbah.Media.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentManagement.Controllers
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

        [Route("list"), HttpGet]
        public async Task<IEnumerable<MediaItem>> List()
            => await _mediaService.ListMediaAsync();

        [Route("upload"), HttpPost]
        public async Task<IEnumerable<Guid>> Upload(IEnumerable<IFormFile> files)
            => await Task.WhenAll(files.Concat(Request.Form.Files).Select(async ent => await _mediaService.PutMediaAsync(ent.OpenReadStream(), ent.FileName, ent.ContentType)));

        [Route("{id}"), HttpGet, AllowAnonymous]
        public async Task<FileResult> GetMedia(Guid id, [FromQuery] GetMediaRequest request)
        {
            var item = await _mediaService.GetMediaItemAsync(id);
            var stream = await _mediaService.GetMediaStreamAsync(id);

            if (!request.IsEmpty)
            {
                // TODO: fix this error:
                // Could not load file or assembly 'ImageSharp, ...'
                // var image = new Image(stream);
                // var resized = image.Resize(new ResizeOptions
                // {
                //     // TODO: scale this
                //     Size = new Size(request.Width ?? image.Width, request.Height ?? image.Height)
                // });
                // using (var resizedStream = new MemoryStream())
                // {
                //     resized.Save(resizedStream);
                //     resizedStream.Seek(0, SeekOrigin.Begin);
                //     return new FileStreamResult(resizedStream, item.ContentType);
                // }
            }

            return new FileStreamResult(stream, item.ContentType);
        }
    }

    public class GetMediaRequest
    {
        public int? Width { get; set; }
        public int? Height { get; set; }

        public bool IsEmpty
            => (!Width.HasValue && !Height.HasValue);
    }
}
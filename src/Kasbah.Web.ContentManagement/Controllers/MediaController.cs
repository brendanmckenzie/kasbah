using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Processing;
using Kasbah.Media;
using Kasbah.Media.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.ContentManagement.Controllers
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
                if (!request.IsEmpty)
                {
                    var image = new Image(stream);
                    var options = new ResizeOptions();
                    options.Mode = ResizeMode.BoxPad;
                    options.Size = new Size(request.Width.Value, request.Height.Value);

                    image.Resize(options);

                    using (var output = new MemoryStream())
                    {
                        image.Save(output);

                        return new FileStreamResult(output, item.ContentType);
                    }
                }
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
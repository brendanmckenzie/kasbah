using System;
using System.IO;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Processing;
using Kasbah.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// TODO: find a way to not have this duplicated in CD and CM
namespace Kasbah.Web.ContentDelivery.Controllers
{
    [Route("media")]
    public class MediaController : Controller
    {
        readonly MediaService _mediaService;

        public MediaController(MediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [Route("{id}"), HttpGet, AllowAnonymous, ResponseCache(Duration = 3600)]
        public async Task<FileResult> GetMedia(Guid id, [FromQuery] GetMediaRequest request)
        {
            var item = await _mediaService.GetMediaItemAsync(id);
            var stream = await _mediaService.GetMediaStreamAsync(id);

            if (!request.IsEmpty)
            {
                try
                {
                    var image = new Image(stream);
                    var resized = image.Resize(new ResizeOptions
                    {
                        Size = CalculateSize(image.Width, image.Height, request.Width, request.Height)
                    });
                    var resizedStream = new MemoryStream();
                    resized.Save(resizedStream);
                    resizedStream.Seek(0, SeekOrigin.Begin);

                    return new FileStreamResult(resizedStream, item.ContentType);
                }
                finally
                {
                    stream.Dispose();
                }
            }

            return new FileStreamResult(stream, item.ContentType);
        }

        Size CalculateSize(int sourceWidth, int sourceHeight, int? destWidth, int? destHeight)
        {
            if (destWidth.HasValue && destHeight.HasValue)
            {
                return new Size(destWidth.Value, destHeight.Value);
            }
            else if (destWidth.HasValue)
            {
                return new Size(destWidth.Value, sourceWidth / sourceHeight * destWidth.Value);
            }
            else if (destHeight.HasValue)
            {
                return new Size(sourceHeight / sourceWidth * destHeight.Value, destHeight.Value);
            }

            return new Size(sourceWidth, sourceHeight);
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

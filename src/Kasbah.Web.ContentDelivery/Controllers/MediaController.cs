using System.Threading.Tasks;
using Kasbah.Media;
using Microsoft.AspNetCore.Mvc;

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

        [Route("")]
        [HttpGet]
        [ResponseCache(Duration = 3600)]
        public async Task<FileResult> GetMedia([FromQuery] GetMediaRequest request)
        {
            var media = await _mediaService.GetMedia(request);

            return new FileStreamResult(media.Stream, media.Item.ContentType);
        }
    }
}

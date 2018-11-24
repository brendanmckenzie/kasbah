using System.Threading.Tasks;
using Kasbah.Media;
using Microsoft.AspNetCore.Mvc;

namespace Kasbah.Web.Controllers
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
        public async Task<IActionResult> GetMedia([FromQuery] GetMediaRequest request)
        {
            var media = await _mediaService.GetMedia(request);

            if (media == null)
            {
                return NotFound();
            }

            return new FileStreamResult(media.Stream, media.Item.ContentType);
        }
    }
}

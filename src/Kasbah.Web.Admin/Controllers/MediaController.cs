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
    public class MediaController
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
            => await Task.WhenAll(files.Select(async ent => await _mediaService.PutMediaAsync(ent.OpenReadStream(), ent.FileName, ent.ContentType)));

    }
}
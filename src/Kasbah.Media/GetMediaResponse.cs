using System.IO;
using Kasbah.Media.Models;

namespace Kasbah.Media
{
    public class GetMediaResponse
    {
        public Stream Stream { get; set; }

        public MediaItem Item { get; set; }
    }
}

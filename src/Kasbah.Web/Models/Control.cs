using Newtonsoft.Json.Linq;

namespace Kasbah.Web.Models
{
    public class Control
    {
        public string Alias { get; set; }

        public JObject Model { get; set; }

        public PlaceholderCollection Placeholders { get; set; }
    }
}

using Kasbah.Content.Attributes;
using Kasbah.Content.Models;

namespace <%= namespace %>.Models
{
    [Icon("badge", "#FF3860")]
    public class WebRoot : Item
    {
        public string GoogleTagManagerCode { get; set; }
    }
}

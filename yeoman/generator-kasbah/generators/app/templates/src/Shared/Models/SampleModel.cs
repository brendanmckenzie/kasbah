using Kasbah.Content.Attributes;
using Kasbah.Content.Models;
using Kasbah.Media.Models;

namespace <%= namespace %>.Models
{
    public class SampleModel : Item
    {
        public NestedObject Hero { get; set; }
        public MediaItem Image { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class NestedObject
    {
        public string Title { get; set; }
        [FieldEditor("longText")]
        public string Body { get; set; }
    }
}

using Kasbah.Content.Models;

namespace Kasbah.Web.Models.Delivery
{
    public class RenderModel
    {
        public string TraceIdentifier { get; set; }

        public Node Node { get; set; }

        public Site Site { get; set; }

        public Node SiteNode { get; set; }

        public ControlRenderModel Head { get; set; }

        public ControlRenderModel Body { get; set; }
    }
}

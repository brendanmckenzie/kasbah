using System.Collections.Generic;
using Kasbah.Content.Models;

namespace Kasbah.Web.Models.Delivery
{
    public class RenderModel
    {
        public Node Node { get; set; }

        public object Content { get; set; }

        public Site Site { get; set; }

        public Node SiteNode { get; set; }

        public string Layout { get; set; }

        public ComponentMap Components { get; set; }

        public class ComponentMap : Dictionary<string, IEnumerable<RenderModel.Component>>
        {
            public ComponentMap()
            {
            }

            public ComponentMap(IDictionary<string, IEnumerable<RenderModel.Component>> dictionary)
                : base(dictionary)
            {
            }
        }

        public class Component
        {
            public string Alias { get; set; }

            public object Model { get; set; }
        }
    }
}

using System.Linq;
using Kasbah.Content.Models;

namespace Kasbah.Web.Models
{
    public class Presentable : Item
    {
        public string Layout { get; set; }

        public ILookup<string, Component> Components { get; set; }
    }

    public class Component
    {
        public string Control { get; set; }
        public Item DataSource { get; set; }
    }
}

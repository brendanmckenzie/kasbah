using System.Linq;
using System.Collections.Generic;
using Kasbah.Content.Models;
using Kasbah.Content.Attributes;

namespace Kasbah.Web.Models
{
    public interface IPresentable
    {
        // TODO: make these attributes apply to sub-classes

        [FieldEditor("kasbah_web:components")]
        ComponentCollection Components { get; set; }
    }

    public abstract class Presentable : Item, IPresentable
    {

        [FieldEditor("kasbah_web:components")]
        public ComponentCollection Components { get; set; }
    }

    public class ComponentCollection : Dictionary<string, ICollection<Component>> { }

    public class Component
    {
        public string Control { get; set; }
        public IDictionary<string, object> Properties { get; set; }
    }
}

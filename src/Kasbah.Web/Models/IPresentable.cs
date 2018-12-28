using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kasbah.Content.Attributes;
using Kasbah.Content.Models;

namespace Kasbah.Web.Models
{
    public interface IPresentable
    {
        string Layout { get; set; }

        // TODO: make these attributes apply to sub-classes
        [FieldEditor("kasbah_web:components")]
        ComponentCollection Components { get; set; }

        Task<object> GetModelAsync();

        Task<ComponentCollection> ListStaticComponentsAsync(KasbahWebContext context);

        Control HeadControl { get; set; }

        Control BodyControl { get; set; }
    }

    public class BasePresentable : Item, IPresentable
    {
        public virtual string Layout { get; set; }

        [FieldEditor("kasbah_web:components")]
        public virtual ComponentCollection Components { get; set; }
        public virtual Control HeadControl { get; set; }
        public virtual Control BodyControl { get; set; }

        public virtual Task<object> GetModelAsync() => Task.FromResult(default(object));

        public virtual Task<ComponentCollection> ListStaticComponentsAsync(KasbahWebContext context) => Task.FromResult(new ComponentCollection());
    }

    public class PlaceholderDefinition
    {
        public string Key { get; set; }

        public IEnumerable<string> AllowedControls { get; set; }
    }

    public class ControlDefinition
    {
        public string Key { get; set; }

        public string Control { get; set; }

        public Type Model { get; set; }

        public IEnumerable<PlaceholderDefinition> Placeholders { get; set; }
    }

    public class Placeholder
    {
        public string Key { get; set; }

        public IEnumerable<Control> Controls { get; set; }
    }

    public class Control
    {
        public string Key { get; set; } // matches `ControlDefinition.Key`

        public object Model { get; set; }

        public IEnumerable<Placeholder> Placeholders { get; set; }
    }
}

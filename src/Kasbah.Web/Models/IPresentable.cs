using Kasbah.Content.Attributes;
using Kasbah.Content.Models;

namespace Kasbah.Web.Models
{
    public interface IPresentable
    {
        // TODO: make these attributes apply to sub-classes
        [FieldEditor("kasbah_web:control")]
        Control Layout { get; set; }
    }

    public class BasePresentable : Item, IPresentable
    {
        [FieldEditor("kasbah_web:control")]
        public virtual Control Layout { get; set; }
    }
}

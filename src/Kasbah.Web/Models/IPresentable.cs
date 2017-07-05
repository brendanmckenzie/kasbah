using Kasbah.Content.Attributes;

namespace Kasbah.Web.Models
{
    public interface IPresentable
    {
        // TODO: make these attributes apply to sub-classes
        [FieldEditor("kasbah_web:components")]
        ComponentCollection Components { get; set; }
    }
}

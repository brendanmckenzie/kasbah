using Kasbah.Content.Attributes;
using Kasbah.Content.Models;

namespace Kasbah.Web.Models
{
    public abstract class Presentable : Item, IPresentable
    {
        [FieldEditor("kasbah_web:components")]
        public ComponentCollection Components { get; set; }
    }
}

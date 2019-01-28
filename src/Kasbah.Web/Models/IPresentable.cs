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

        [FieldEditor("kasbah_web:control")]
        Control HeadControl { get; set; }

        [FieldEditor("kasbah_web:control")]
        Control BodyControl { get; set; }

        Task<object> GetModelAsync();
    }

    public class BasePresentable : Item, IPresentable
    {
        public virtual string Layout { get; set; }

        [FieldEditor("kasbah_web:components")]
        public virtual ComponentCollection Components { get; set; }

        [FieldEditor("kasbah_web:control")]
        public virtual Control HeadControl { get; set; }

        [FieldEditor("kasbah_web:control")]
        public virtual Control BodyControl { get; set; }

        public virtual Task<object> GetModelAsync() => Task.FromResult(default(object));

        public virtual Task<ComponentCollection> ListStaticComponentsAsync(KasbahWebContext context) => Task.FromResult(new ComponentCollection());
    }
}

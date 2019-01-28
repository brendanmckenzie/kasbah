using System.Collections.Generic;
using System.Linq;

namespace Kasbah.Web.Models
{
    public abstract class ComponentBase
    {
        public abstract string Alias { get; }

        public virtual IEnumerable<PlaceholderDefinition> Placeholders { get; } = Enumerable.Empty<PlaceholderDefinition>();
    }
}

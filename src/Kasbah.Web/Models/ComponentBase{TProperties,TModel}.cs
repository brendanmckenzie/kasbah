using System.Threading.Tasks;

namespace Kasbah.Web.Models
{
    public abstract class ComponentBase<TProperties, TModel> : ComponentBase
    {
        public virtual Task<TModel> GetModelAsync(KasbahWebContext context, TProperties properties, IPresentable content)
            => Task.FromResult(default(TModel));
    }
}

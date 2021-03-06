﻿using System.Threading.Tasks;

namespace Kasbah.Web.Models
{
    public abstract class ComponentBase<TModel> : ComponentBase
    {
        public virtual Task<TModel> GetModelAsync(KasbahWebContext context, TModel model, IPresentable content)
            => Task.FromResult(model);
    }
}

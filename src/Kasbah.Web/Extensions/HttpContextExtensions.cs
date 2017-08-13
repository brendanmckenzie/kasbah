using Microsoft.AspNetCore.Http;

namespace Kasbah.Web
{
    public static class HttpContextExtensions
    {
        public static KasbahWebContext GetKasbahWebContext(this HttpContext context)
            => context.Items[KasbahWebContext.Key] as KasbahWebContext;
    }
}

using System;
using Microsoft.AspNetCore.Http;

namespace Kasbah.Web.ContentDelivery.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetCurrentProfileId(this HttpContext context)
        {
            return new Guid(Convert.FromBase64String(context.Items["user:profile"] as string));
        }
    }
}
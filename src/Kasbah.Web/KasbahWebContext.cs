using System;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Kasbah.Web
{
    public class KasbahWebContext
    {
        public KasbahWebApplication WebApplication { get; set; }

        public HttpContext HttpContext { get; set; }

        public ContentService ContentService { get; set; }

        public TypeRegistry TypeRegistry { get; set; }

        public TypeMapper TypeMapper { get; set; }

        public SiteRegistry SiteRegistry { get; set; }

        public Guid RequestId { get; set; } = Guid.NewGuid();

        public Site Site { get; set; }

        public Node Node { get; set; }

        public Guid Profile { get; set; }
    }
}

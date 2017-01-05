using System;
using Kasbah.Content;
using Kasbah.Content.Models;
using Kasbah.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Kasbah.Web.Public
{
    public class KasbahWebContext
    {
        public KasbahWebApplication WebApplication { get; set; }
        public HttpContext HttpContext { get; set; }
        public ContentService ContentService { get; set; }
        public Guid RequestId { get; set; } = Guid.NewGuid();
        public Site Site { get; set; }
        public Node Node { get; set; }
    }
}
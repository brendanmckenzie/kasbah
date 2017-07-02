using Kasbah.Content;
using Kasbah.Web;
using <%= namespace %>.Models;

namespace <%= namespace %>
{
    public class WebRegistration : KasbahWebRegistration
    {
        public override void RegisterSites(SiteRegistry siteRegistry)
        {
            siteRegistry.RegisterSite(config =>
            {
                config.Alias = "<%= alias %>";
                config.Domains = new[] { "localhost:5001" };
                config.ContentRoot = new[] { "sites", "<%= alias %>", "home" };
            });
        }
        public override void RegisterTypes(TypeRegistry typeRegistry)
        {
            typeRegistry.Register<WebRoot>();
            typeRegistry.Register<HomePage>(config =>
            {
                config
                    .FieldCategory(nameof(HomePage.Title), "Content")
                    .FieldCategory(nameof(HomePage.Body), "Content")
                    .SetOption("view", "HomePage");
            });
            typeRegistry.Register<SampleModel>();
        }
    }
}

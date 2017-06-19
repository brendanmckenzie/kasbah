using Kasbah.Content;

namespace Kasbah.Web
{
    public abstract class KasbahWebRegistration
    {
        public abstract void RegisterSites(SiteRegistry siteRegistry);
        public abstract void RegisterTypes(TypeRegistry typeRegistry);
        public abstract void RegisterComponents(ComponentRegistry componentRegistry);
    }
}

using Kasbah.Content;

namespace Kasbah.Web
{
    public abstract class KasbahWebRegistration
    {
        public virtual void RegisterSites(SiteRegistry siteRegistry)
        {
        }

        public virtual void RegisterTypes(TypeRegistry typeRegistry)
        {
        }

        public virtual void RegisterComponents(ComponentRegistry componentRegistry)
        {
        }
    }
}

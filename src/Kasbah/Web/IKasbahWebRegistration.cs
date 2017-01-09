using Kasbah.Content;

namespace Kasbah.Web
{
    public interface IKasbahWebRegistration
    {
        void RegisterSites(SiteRegistry siteRegistry);
        void RegisterTypes(TypeRegistry typeRegistry);
    }
}
namespace Kasbah.Web.ContentDelivery.Models
{
    public interface IViewResolver
    {
        string GetView(KasbahWebContext context);
    }
}

namespace Kasbah.Web.Delivery.Models
{
    public interface IViewResolver
    {
        string GetView(KasbahWebContext context);
    }
}

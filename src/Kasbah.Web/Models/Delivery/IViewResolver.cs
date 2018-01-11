namespace Kasbah.Web.Models.Delivery
{
    public interface IViewResolver
    {
        string GetView(KasbahWebContext context);
    }
}

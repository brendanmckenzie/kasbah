using System.Threading.Tasks;
using Kasbah.Content.Models;

namespace Kasbah.Content.Events
{
    public interface IOnContentPublished
    {
         Task ContentPublishedAsync(Node node);
    }
}

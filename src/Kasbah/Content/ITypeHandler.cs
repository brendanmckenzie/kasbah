using System;
using System.Threading.Tasks;

namespace Kasbah.Content
{
    public interface ITypeHandler
    {
        string DefaultEditor { get; }

        bool CanConvert(Type type);

        Task<object> ConvertAsync(object source);
    }
}

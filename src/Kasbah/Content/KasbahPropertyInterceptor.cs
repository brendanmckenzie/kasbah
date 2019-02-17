using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using StackExchange.Profiling;

namespace Kasbah.Content
{
    delegate Task<object> MapPropertyDelegate(object source, PropertyInfo property, TypeMapperContext context);

    class KasbahPropertyInterceptor : IInterceptor
    {
        readonly MapPropertyDelegate _mapProperty;
        readonly IDictionary<string, object> _data;
        readonly TypeMapperContext _context;

        public KasbahPropertyInterceptor(MapPropertyDelegate mapProperty, IDictionary<string, object> data, TypeMapperContext context)
        {
            _mapProperty = mapProperty;
            _data = data;
            _context = context;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.StartsWith("get_"))
            {
                var propertyName = invocation.Method.Name.Substring(4);

                if (_data.ContainsKey(propertyName))
                {
                    using (MiniProfiler.Current.Step($"Mapping lazy property ('{propertyName}')"))
                    {
                        var property = invocation.TargetType.GetProperty(propertyName);

                        invocation.ReturnValue = _mapProperty.Invoke(_data[propertyName], property, _context).Result;
                    }
                }
            }
        }
    }
}

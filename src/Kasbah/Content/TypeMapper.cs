using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kasbah.Content
{
    public class TypeMapper
    {
        // TODO: implement linking to other objects
        public object MapType(IDictionary<string, object> data, string typeName)
        {
            var type = Type.GetType(typeName);
            var typeInfo = type.GetTypeInfo();

            var ret = Activator.CreateInstance(type);
            foreach (var property in typeInfo.GetProperties())
            {
                var key = property.Name;
                if (data.ContainsKey(key))
                {
                    var value = data[key];
                    if (value.GetType() == property.PropertyType)
                    {
                        property.SetValue(ret, value);
                    }
                    else
                    {
                        // property type mismatch
                        Console.WriteLine($"Unable to map {key}, source type: {value.GetType().Name}, destination type: {property.PropertyType.Name}");
                    }
                }
            }

            return ret;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Kasbah.Content
{
    public class TypeMapper
    {
        public TypeMapper()
        {
        }

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
                    if (value == null) { continue; }

                    // TODO: handle nested objects and slight datatype mismatches (int64/int32, decimal/double, etc...)
                    if (value.GetType() == property.PropertyType)
                    {
                        property.SetValue(ret, value);
                    }
                    else if (value.GetType().IsAssignableFrom(typeof(IDictionary)))
                    {
                        // TODO: handle nested object
                    }
                    else if (value.GetType() == typeof(string))
                    {
                        Guid id;
                        if (Guid.TryParse((string)value, out id))
                        {
                            // TODO: handle referenced object (Guid)
                        }
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
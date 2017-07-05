using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kasbah.Provider.Npgsql
{
    internal static class TypeSystem
    {
        internal static Type GetElementType(Type type)
        {
            var ienum = FindIEnumerable(type);

            if (ienum == null)
            {
                return type;
            }

            return ienum.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            if (type == null || type == typeof(string))
            {
                return null;
            }

            if (type.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());
            }

            if (typeInfo.IsGenericType)
            {
                foreach (Type arg in type.GetGenericArguments())
                {
                    var ienum = typeof(IEnumerable<>).MakeGenericType(arg);

                    if (ienum.IsAssignableFrom(type))
                    {
                        return ienum;
                    }
                }
            }

            var ifaces = type.GetInterfaces();

            if (ifaces.Any())
            {
                foreach (var iface in ifaces)
                {
                    var ienum = FindIEnumerable(iface);

                    if (ienum != null)
                    {
                        return ienum;
                    }
                }
            }

            if (typeInfo.BaseType != null && typeInfo.BaseType != typeof(object))
            {
                return FindIEnumerable(typeInfo.BaseType);
            }

            return null;
        }
    }
}

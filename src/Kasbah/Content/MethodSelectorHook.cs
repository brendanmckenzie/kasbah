using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace Kasbah.Content
{
    public class MethodSelectorHook : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            => methodInfo.Name.StartsWith("get_");

    }
}

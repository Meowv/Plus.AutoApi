using System;
using System.Linq;
using System.Reflection;

namespace Plus.AutoApi.Helpers
{
    internal static class ReflectionHelper
    {
        public static TAttribute GetSingleAttributeOrDefault<TAttribute>(TypeInfo typeInfo) where TAttribute : Attribute
        {
            var attributeType = typeof(TAttribute);

            if (typeInfo.IsDefined(attributeType, true))
            {
                return typeInfo.GetCustomAttributes(attributeType, true).Cast<TAttribute>().First();
            }
            else
            {
                foreach (var item in typeInfo.ImplementedInterfaces)
                {
                    var attribute = GetSingleAttributeOrDefault<TAttribute>(item.GetTypeInfo());

                    if (attribute != null)
                        return attribute;
                }
            }

            return null;
        }

        public static TAttribute GetSingleAttributeOrNull<TAttribute>(this MemberInfo memberInfo, bool inherit = true)
            where TAttribute : Attribute
        {
            if (memberInfo == null)
                throw new ArgumentNullException(nameof(memberInfo));

            var attributes = memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).ToArray();

            if (attributes.Length > 0)
                return (TAttribute)attributes.First();

            return default;
        }

        public static TAttribute GetSingleAttributeOfTypeOrBaseTypesOrNull<TAttribute>(this Type type, bool inherit = true)
            where TAttribute : Attribute
        {
            var attribute = type.GetTypeInfo().GetSingleAttributeOrNull<TAttribute>();
            if (attribute != null)
                return attribute;

            if (type.GetTypeInfo().BaseType == null)
                return null;

            return type.GetTypeInfo().BaseType.GetSingleAttributeOfTypeOrBaseTypesOrNull<TAttribute>(inherit);
        }
    }
}
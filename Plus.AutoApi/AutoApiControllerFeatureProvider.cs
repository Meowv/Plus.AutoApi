using Microsoft.AspNetCore.Mvc.Controllers;
using Plus.AutoApi.Attributes;
using Plus.AutoApi.Helpers;
using System.Reflection;

namespace Plus.AutoApi
{
    public class AutoApiControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            var type = typeInfo.AsType();

            if (!typeof(IAutoApi).IsAssignableFrom(type) || typeInfo.IsNotPublic || type.IsAbstract || typeInfo.IsGenericType)
            {
                return false;
            }

            var attribute = ReflectionHelper.GetSingleAttributeOrDefault<AutoApiAttribute>(typeInfo);

            if (attribute == null || attribute.Disabled)
            {
                return false;
            }

            return true;
        }
    }
}
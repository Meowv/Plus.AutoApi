using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Plus.AutoApi.Attributes;
using Plus.AutoApi.Extensions;
using Plus.AutoApi.Helpers;
using System;
using System.Linq;
using System.Reflection;

namespace Plus.AutoApi
{
    public class AutoApiConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var type = controller.ControllerType.AsType();
                var attribute = ReflectionHelper.GetSingleAttributeOrDefault<AutoApiAttribute>(type.GetTypeInfo());

                if (typeof(IAutoApi).GetTypeInfo().IsAssignableFrom(type))
                {
                    controller.ControllerName = controller.ControllerName.RemoveSuffix(PlusConsts.ControllerSuffixes.ToArray());

                    ConfigureArea(controller, attribute);

                    ConfigureAutoApi(controller, attribute);
                }
                else
                {
                    if (attribute != null)
                    {
                        ConfigureArea(controller, attribute);

                        ConfigureAutoApi(controller, attribute);
                    }
                }
            }
        }

        private void ConfigureArea(ControllerModel controller, AutoApiAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentException(nameof(attribute));

            if (!controller.RouteValues.ContainsKey("area"))
            {
                if (!string.IsNullOrEmpty(attribute.AreaName))
                {
                    controller.RouteValues["area"] = attribute.AreaName;
                }
                else if (!string.IsNullOrEmpty(PlusConsts.DefaultAreaName))
                {
                    controller.RouteValues["area"] = PlusConsts.DefaultAreaName;
                }
            }
        }

        private void ConfigureAutoApi(ControllerModel controller, AutoApiAttribute attribute)
        {
            ConfigureApiExplorer(controller);
            ConfigureSelector(controller, attribute);
            ConfigureParameters(controller);
        }

        private void ConfigureApiExplorer(ControllerModel controller)
        {
            if (string.IsNullOrEmpty(controller.ApiExplorer.GroupName))
            {
                controller.ApiExplorer.GroupName = controller.ControllerName;
            }

            if (controller.ApiExplorer.IsVisible == null)
            {
                controller.ApiExplorer.IsVisible = true;
            }

            foreach (var action in controller.Actions)
            {
                ConfigureApiExplorer(action);
            }
        }

        private void ConfigureApiExplorer(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible == null)
            {
                action.ApiExplorer.IsVisible = true;
            }
        }

        private void ConfigureSelector(ControllerModel controller, AutoApiAttribute attribute)
        {
            if (controller.Selectors.Any(selector => selector.AttributeRouteModel != null))
            {
                return;
            }

            var areaName = string.Empty;

            if (attribute != null)
            {
                areaName = attribute.AreaName;
            }

            foreach (var action in controller.Actions)
            {
                ConfigureSelector(areaName, controller.ControllerName, action);
            }
        }

        private void ConfigureSelector(string areaName, string controllerName, ActionModel action)
        {
            if (!action.Selectors.Any() || action.Selectors.Any(x => !x.ActionConstraints.Any()))
            {
                AddApplicationServiceSelector(areaName, controllerName, action);
            }
            else
            {
                NormalizeSelectorRoutes(areaName, controllerName, action);
            }
        }

        private void AddApplicationServiceSelector(string areaName, string controllerName, ActionModel action)
        {
            var verb = GetHttpVerb(action);

            action.ActionName = GetRestFulActionName(action.ActionName);

            var appServiceSelectorModel = action.Selectors[0];

            if (appServiceSelectorModel.AttributeRouteModel == null)
            {
                appServiceSelectorModel.AttributeRouteModel = CreateActionRouteModel(areaName, controllerName, action);
            }

            if (!appServiceSelectorModel.ActionConstraints.Any())
            {
                appServiceSelectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { verb }));
                switch (verb)
                {
                    case "GET":
                        appServiceSelectorModel.EndpointMetadata.Add(new HttpGetAttribute());
                        break;
                    case "POST":
                        appServiceSelectorModel.EndpointMetadata.Add(new HttpPostAttribute());
                        break;
                    case "PUT":
                        appServiceSelectorModel.EndpointMetadata.Add(new HttpPutAttribute());
                        break;
                    case "DELETE":
                        appServiceSelectorModel.EndpointMetadata.Add(new HttpDeleteAttribute());
                        break;
                    default:
                        throw new Exception($"Unsupported http verb: {verb}.");
                }
            }
        }

        private void NormalizeSelectorRoutes(string areaName, string controllerName, ActionModel action)
        {
            action.ActionName = GetRestFulActionName(action.ActionName);

            foreach (var selector in action.Selectors)
            {
                selector.AttributeRouteModel = selector.AttributeRouteModel == null ?
                     CreateActionRouteModel(areaName, controllerName, action) :
                     AttributeRouteModel.CombineAttributeRouteModel(CreateActionRouteModel(areaName, controllerName, action), selector.AttributeRouteModel);
            }
        }

        private static string GetHttpVerb(ActionModel action)
        {
            var getValueSuccess = PlusConsts.AssemblyAutoApiOptions.TryGetValue(action.Controller.ControllerType.Assembly, out AssemblyAutoApiOptions assemblyAutoApiOptions);

            if (getValueSuccess && !string.IsNullOrWhiteSpace(assemblyAutoApiOptions?.HttpVerb))
            {
                return assemblyAutoApiOptions.HttpVerb;
            }

            var verbKey = action.ActionName.GetPascalOrCamelCaseFirstWord().ToLower();

            return PlusConsts.HttpVerbs.ContainsKey(verbKey) ? PlusConsts.HttpVerbs[verbKey] : PlusConsts.DefaultHttpVerb;
        }

        private string GetRestFulActionName(string actionName)
        {
            var name = PlusConsts.GetRestFulActionName?.Invoke(actionName);
            if (name != null)
            {
                return name;
            }

            actionName = actionName.RemoveSuffix(PlusConsts.ActionSuffixes.ToArray());

            var verbKey = actionName.GetPascalOrCamelCaseFirstWord().ToLower();
            if (PlusConsts.HttpVerbs.ContainsKey(verbKey))
            {
                if (actionName.Length == verbKey.Length)
                {
                    return "";
                }
                else
                {
                    return actionName.Substring(verbKey.Length);
                }
            }
            else
            {
                return actionName;
            }
        }

        private AttributeRouteModel CreateActionRouteModel(string areaName, string controllerName, ActionModel action)
        {
            var apiPreFix = GetApiPreFix(action);
            var routeStr = $"{apiPreFix}/{areaName}/{controllerName}/{action.ActionName}".Replace("//", "/");

            return new AttributeRouteModel(new RouteAttribute(routeStr));
        }

        private static string GetApiPreFix(ActionModel action)
        {
            var getValueSuccess = PlusConsts.AssemblyAutoApiOptions.TryGetValue(action.Controller.ControllerType.Assembly, out AssemblyAutoApiOptions assemblyAutoApiOptions);

            if (getValueSuccess && !string.IsNullOrWhiteSpace(assemblyAutoApiOptions?.ApiPrefix))
            {
                return assemblyAutoApiOptions.ApiPrefix;
            }

            return PlusConsts.DefaultApiPreFix;
        }

        private void ConfigureParameters(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                foreach (var para in action.Parameters)
                {
                    if (para.BindingInfo != null)
                    {
                        continue;
                    }

                    if (!TypeHelper.IsPrimitiveExtendedIncludingNullable(para.ParameterInfo.ParameterType))
                    {
                        if (CanUseFormBodyBinding(action, para))
                        {
                            para.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                        }
                    }
                }
            }
        }

        private bool CanUseFormBodyBinding(ActionModel action, ParameterModel parameter)
        {
            if (PlusConsts.FormBodyBindingIgnoredTypes.Any(t => t.IsAssignableFrom(parameter.ParameterInfo.ParameterType)))
            {
                return false;
            }

            foreach (var selector in action.Selectors)
            {
                if (selector.ActionConstraints == null)
                {
                    continue;
                }

                foreach (var actionConstraint in selector.ActionConstraints)
                {
                    if (!(actionConstraint is HttpMethodActionConstraint httpMethodActionConstraint))
                    {
                        continue;
                    }

                    if (httpMethodActionConstraint.HttpMethods.All(x => x.IsIn("GET", "DELETE", "TRACE", "HEAD")))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
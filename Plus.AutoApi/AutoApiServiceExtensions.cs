using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Plus.AutoApi
{
    public static class AutoApiServiceExtensions
    {
        public static IServiceCollection AddAutoApi(this IServiceCollection services)
        {
            return AddAutoApi(services, new AutoApiOptions());
        }

        public static IServiceCollection AddAutoApi(this IServiceCollection services, Action<AutoApiOptions> optionsAction)
        {
            var options = new AutoApiOptions();

            optionsAction?.Invoke(options);

            return AddAutoApi(services, options);
        }

        private static IServiceCollection AddAutoApi(this IServiceCollection services, AutoApiOptions options)
        {
            if (options == null)
            {
                throw new ArgumentException(nameof(options));
            }

            options.Valid();

            PlusConsts.DefaultAreaName = options.DefaultAreaName;
            PlusConsts.DefaultHttpVerb = options.DefaultHttpVerb;
            PlusConsts.DefaultApiPreFix = options.DefaultApiPrefix;
            PlusConsts.ControllerSuffixes = options.RemoveControllerSuffixes;
            PlusConsts.ActionSuffixes = options.RemoveActionSuffixes;
            PlusConsts.FormBodyBindingIgnoredTypes = options.FormBodyBindingIgnoredTypes;
            PlusConsts.GetRestFulActionName = options.GetRestFulActionName;
            PlusConsts.AssemblyAutoApiOptions = options.AssemblyAutoApiOptions;

            var partManager = services.GetSingletonInstanceOrNull<ApplicationPartManager>();
            if (partManager == null)
            {
                throw new InvalidOperationException("\"AddAutoApi\" must be after \"AddMvc\".");
            }

            partManager.FeatureProviders.Add(new AutoApiControllerFeatureProvider());

            services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new AutoApiConvention());
            });

            return services;
        }

        private static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
        {
            return (T)services.FirstOrDefault(d => d.ServiceType == typeof(T))?.ImplementationInstance;
        }
    }
}
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Plus.AutoApi
{
    public class AutoApiOptions
    {
        public AutoApiOptions()
        {
            RemoveControllerSuffixes = new List<string>() { "AppService", "ApplicationService" };
            RemoveActionSuffixes = new List<string>() { "Async" };
            FormBodyBindingIgnoredTypes = new List<Type>() { typeof(IFormFile) };
            DefaultHttpVerb = "POST";
            DefaultApiPrefix = "api";
            AssemblyAutoApiOptions = new Dictionary<Assembly, AssemblyAutoApiOptions>();
        }

        public string DefaultHttpVerb { get; set; }

        public string DefaultAreaName { get; set; }

        public string DefaultApiPrefix { get; set; }

        public List<string> RemoveControllerSuffixes { get; set; }

        public List<string> RemoveActionSuffixes { get; set; }

        public List<Type> FormBodyBindingIgnoredTypes { get; set; }

        public Func<string, string> GetRestFulActionName { get; set; }

        public Dictionary<Assembly, AssemblyAutoApiOptions> AssemblyAutoApiOptions { get; }

        public void Valid()
        {
            if (string.IsNullOrEmpty(DefaultHttpVerb))
            {
                throw new ArgumentException($"{nameof(DefaultHttpVerb)} can not be empty.");
            }

            if (string.IsNullOrEmpty(DefaultAreaName))
            {
                DefaultAreaName = string.Empty;
            }

            if (string.IsNullOrEmpty(DefaultApiPrefix))
            {
                DefaultApiPrefix = string.Empty;
            }

            if (FormBodyBindingIgnoredTypes == null)
            {
                throw new ArgumentException($"{nameof(FormBodyBindingIgnoredTypes)} can not be null.");
            }

            if (RemoveControllerSuffixes == null)
            {
                throw new ArgumentException($"{nameof(RemoveControllerSuffixes)} can not be null.");
            }
        }

        public void AddAssemblyOptions(Assembly assembly, string apiPreFix = null, string httpVerb = null)
        {
            if (assembly == null)
            {
                throw new ArgumentException($"{nameof(assembly)} can not be null.");
            }

            AssemblyAutoApiOptions[assembly] = new AssemblyAutoApiOptions(apiPreFix, httpVerb);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Plus.AutoApi
{
    public static class PlusConsts
    {
        static PlusConsts()
        {
            HttpVerbs = new Dictionary<string, string>()
            {
                ["add"] = "POST",
                ["create"] = "POST",
                ["post"] = "POST",
                ["insert"] = "POST",

                ["get"] = "GET",
                ["find"] = "GET",
                ["fetch"] = "GET",
                ["query"] = "GET",

                ["update"] = "PUT",
                ["put"] = "PUT",

                ["delete"] = "DELETE",
                ["remove"] = "DELETE"
            };
        }

        public static string DefaultHttpVerb { get; set; }

        public static string DefaultAreaName { get; set; }

        public static string DefaultApiPreFix { get; set; }

        public static List<string> ControllerSuffixes { get; set; }

        public static List<string> ActionSuffixes { get; set; }

        public static List<Type> FormBodyBindingIgnoredTypes { get; set; }

        public static Dictionary<string, string> HttpVerbs { get; set; }

        public static Func<string, string> GetRestFulActionName { get; set; }

        public static Dictionary<Assembly, AssemblyAutoApiOptions> AssemblyAutoApiOptions { get; set; }
    }
}
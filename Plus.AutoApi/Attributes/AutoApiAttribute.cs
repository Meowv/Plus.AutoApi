using System;

namespace Plus.AutoApi.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class AutoApiAttribute : Attribute
    {
        public string AreaName { get; set; }

        public bool Disabled { get; set; } = false;
    }
}
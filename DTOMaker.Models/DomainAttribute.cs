using System;

namespace DTOMaker.Models
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class DomainAttribute : Attribute
    {
        public DomainAttribute()
        {
        }

        public string Version => ThisAssembly.AssemblyFileVersion;
    }
}
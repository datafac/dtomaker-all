using System;

namespace DTOMaker.Models;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class NameAttribute : Attribute
{
    public readonly string Name;

    public NameAttribute(string name)
    {
        Name = name;
    }
}

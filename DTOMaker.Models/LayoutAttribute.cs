using System;

namespace DTOMaker.Models;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class LayoutAttribute : Attribute
{
    public readonly LayoutMethod LayoutMethod;
    public LayoutAttribute(LayoutMethod layoutMethod)
    {
        LayoutMethod = layoutMethod;
    }
}


using System;

namespace DTOMaker.Models;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class LayoutAttribute : Attribute
{
    public readonly LayoutMethod LayoutMethod;
    public readonly int BlockLength;
    public LayoutAttribute(LayoutMethod layoutMethod, int blockLength = 0)
    {
        BlockLength = blockLength;
        LayoutMethod = layoutMethod;
    }
}


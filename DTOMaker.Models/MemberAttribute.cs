using System;

namespace DTOMaker.Models;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class MemberAttribute : Attribute
{
    public readonly int Sequence;
    public readonly string? TypeConverterName;

    public MemberAttribute(int sequence, string? typeConverterName = null)
    {
        Sequence = sequence;
        TypeConverterName = typeConverterName;
    }

}

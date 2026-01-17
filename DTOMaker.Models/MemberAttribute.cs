using System;

namespace DTOMaker.Models;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class MemberAttribute : Attribute
{
    public readonly int Sequence;
    public readonly NativeType NativeType;
    public readonly string? ConverterFullName;

    public MemberAttribute(int sequence)
    {
        Sequence = sequence;
        NativeType = NativeType.Undefined;
        ConverterFullName = null;
    }

    public MemberAttribute(int sequence, NativeType nativeType, string converterFullName)
    {
        Sequence = sequence;
        NativeType = nativeType;
        ConverterFullName = converterFullName;
    }
}

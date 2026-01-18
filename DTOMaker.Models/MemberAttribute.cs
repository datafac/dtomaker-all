using System;

namespace DTOMaker.Models;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class MemberAttribute : Attribute
{
    public readonly int Sequence;
    public readonly NativeType NativeType;
    public readonly Type? ConverterType;
    public readonly string? ConverterFullName;

    public MemberAttribute(int sequence)
    {
        Sequence = sequence;
        NativeType = NativeType.Undefined;
        ConverterType = null;
        ConverterFullName = null;
    }

    public MemberAttribute(int sequence, NativeType nativeType, Type converterType)
    {
        Sequence = sequence;
        NativeType = nativeType;
        ConverterType = converterType;
        ConverterFullName = converterType.FullName;
    }
}

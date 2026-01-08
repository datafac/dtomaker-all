using System;

namespace DTOMaker.Models;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class MemberAttribute : Attribute
{
    public readonly int Sequence;
    public readonly NativeType NativeType;
    public readonly string? StructConverter;

    public MemberAttribute(int sequence)
    {
        Sequence = sequence;
        StructConverter = null;
    }

    public MemberAttribute(int sequence, NativeType nativeType, string structConverter)
    {
        Sequence = sequence;
        NativeType = nativeType;
        StructConverter = structConverter;
    }
}

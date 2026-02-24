using Microsoft.CodeAnalysis;

namespace DTOMaker.SrcGen.Core;

public record BitAddress
{
    public int Instance { get; init; }
    public int Position { get; init; }
    public int FieldOffset { get; init; }
    public int FieldLength { get; init; }
    public BitAddress(int instance, int position, int fieldOffset, int fieldLength)
    {
        Instance = instance;
        Position = position;
        FieldOffset = fieldOffset;
        FieldLength = fieldLength;
    }
}
public sealed record OutputMember
{
    public Location Location { get; init; } = Location.None;
    public string Name { get; init; } = string.Empty;
    public int Sequence { get; init; }
    public TypeFullName MemberType { get; init; }
    public MemberKind Kind { get; init; }
    public bool IsCustom { get; init; }
    public bool IsNullable { get; init; }
    public ObsoleteInformation? ObsoleteInfo { get; init; }
    public bool IsObsolete => ObsoleteInfo is not null;
    public int FieldOffset { get; init; }
    public int FieldLength { get; init; }
    public BitAddress? NullAddress { get; init; }
    public bool IsBigEndian { get; init; }
    public string? ConverterSpace { get; init; }
    public string? ConverterName { get; init; }
    public string? FieldJsonName { get; init; }
    public EquatableArray<Diagnostic> Diagnostics { get; init; } = EquatableArray<Diagnostic>.Empty;
}

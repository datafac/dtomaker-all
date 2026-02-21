using DTOMaker.SrcGen.Core;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public record AddFieldCommand : FieldCommand
{
    public string FieldName { get; init; } = string.Empty;
    public NativeType NativeType { get; init; }
    public bool Nullable { get; init; }
    public bool BigEndian { get; init; }

    public AddFieldCommand(int sequence, string fieldName, NativeType fieldType, bool nullable, bool bigEndian = false) : base(sequence)
    {
        FieldName = fieldName;
        NativeType = fieldType;
        Nullable = nullable;
        BigEndian = bigEndian;
    }
}

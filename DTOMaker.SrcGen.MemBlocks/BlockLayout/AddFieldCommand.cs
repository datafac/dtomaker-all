namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public record AddFieldCommand : FieldCommand
{
    public string FieldName { get; init; } = string.Empty;
    public FieldType FieldType { get; init; }
    public bool Nullable { get; init; }
    public bool BigEndian { get; init; }

    public AddFieldCommand(int sequence, string fieldName, FieldType fieldType, bool nullable, bool bigEndian = false) : base(sequence)
    {
        FieldName = fieldName;
        FieldType = fieldType;
        Nullable = nullable;
        BigEndian = bigEndian;
    }
}

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public record FieldDef
{
    public int BlockOffset { get; init; }
    public int FieldLength { get; init; }
    public string FieldName { get; init; } = string.Empty;
    public FieldType FieldType { get; init; }
    public bool Nullable { get; init; }
    public bool BigEndian { get; init; }
}

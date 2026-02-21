using DTOMaker.SrcGen.Core;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public record FieldDef
{
    public int Sequence { get; init; }
    public int FieldOffset { get; init; }
    public int FieldLength { get; init; }
    public string FieldName { get; init; } = string.Empty;
}

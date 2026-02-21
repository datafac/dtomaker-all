using DTOMaker.SrcGen.Core;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public record FieldDef
{
    public int BlockOffset { get; init; }
    public int FieldLength { get; init; }
    public string FieldName { get; init; } = string.Empty;
    public NativeType NativeType { get; init; }
    public bool Nullable { get; init; }
    public bool BigEndian { get; init; }
}

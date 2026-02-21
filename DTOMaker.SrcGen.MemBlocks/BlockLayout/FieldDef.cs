using DTOMaker.SrcGen.Core;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public record FieldDef
{
    public int Sequence { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Offset { get; init; }
    public int Length { get; init; }
}

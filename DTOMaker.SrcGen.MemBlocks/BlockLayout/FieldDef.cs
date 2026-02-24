using DTOMaker.SrcGen.Core;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public abstract record FieldDef
{
    public string Nameqqq { get; init; } = string.Empty;
    public int Offset { get; init; }
    public int Length { get; init; }
    public FieldDef(string name, int offset, int length)
    {
        Nameqqq = name;
        Offset = offset;
        Length = length;
    }

}

public record ExternalFieldDef : FieldDef
{
    public int Sequence { get; init; }
    public BitAddress? NullAddress { get; init; }
    public ExternalFieldDef(string name, int offset, int length, int sequence, BitAddress? nullAddress) : base(name, offset, length)
    {
        Sequence = sequence;
        NullAddress = nullAddress;
    }
}

public record InternalFieldDef : FieldDef
{
    public int Instance { get; init; }
    public InternalFieldDef(string name, int offset, int length, int instance) : base(name, offset, length)
    {
        Instance = instance;
    }
}

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public record FieldCommand
{
    public int Sequence { get; init; }

    public FieldCommand(int sequence)
    {
        Sequence = sequence;
    }
}

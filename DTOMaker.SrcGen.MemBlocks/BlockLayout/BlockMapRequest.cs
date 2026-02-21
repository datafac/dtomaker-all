using DTOMaker.SrcGen.Core;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public readonly struct BlockMapRequest
{
    public readonly int Sequence;
    public readonly string FieldName;
    public readonly NativeType NativeType;
    public BlockMapRequest(int sequence, string fieldName, NativeType nativeType)
    {
        Sequence = sequence;
        FieldName = fieldName;
        NativeType = nativeType;
    }
}
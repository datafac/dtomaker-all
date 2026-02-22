using DTOMaker.SrcGen.Core;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public readonly struct BlockMapRequest
{
    public readonly int Sequence;
    public readonly string FieldName;
    public readonly MemberKind MemberKind;
    public readonly NativeType NativeType;
    public readonly bool IsFlagsByte;
    public BlockMapRequest(int sequence, string fieldName, MemberKind memberKind, NativeType nativeType, bool isFlagsByte = false)
    {
        Sequence = sequence;
        FieldName = fieldName;
        MemberKind = memberKind;
        NativeType = nativeType;
        IsFlagsByte = isFlagsByte;
    }
}
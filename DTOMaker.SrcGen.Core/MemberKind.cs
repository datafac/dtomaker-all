namespace DTOMaker.SrcGen.Core
{
    public enum GeneratorId
    {
        Unknown,
        JsonSystemText,
        JsonNewtonSoft,
        MsgPack2,
        MemBlocks,
    }

    public enum MemberKind
    {
        Unknown,
        Native,
        Entity,
        Binary,
        String,
    }
}

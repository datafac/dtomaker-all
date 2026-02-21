namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public enum FieldType
{
    None = 0,
    Bool01 = 17,
    SInt01 = 18,
    UInt01 = 19,
    Real02 = 33,
    SInt02 = 34,
    UInt02 = 35,
    Char02 = 36,
    Real04 = 49,
    SInt04 = 50,
    UInt04 = 51,
    Real08 = 65,
    SInt08 = 66,
    UInt08 = 67,
    Deci10 = 81,
    SInt10 = 82,
    UInt10 = 83,
    Guid10 = 84,
    String = 241,
    Binary = 242,
    RawB10 = 95,    // 16 bytes
    RawB20 = 111,   // 32 bytes
    RawB40 = 127,   // 64 bytes
    RawB80 = 143,   // 128 bytes
}

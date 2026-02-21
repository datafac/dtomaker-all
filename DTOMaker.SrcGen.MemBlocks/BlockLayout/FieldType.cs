namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public enum FieldType
{
    Undefined = 0,
    Boolean = 17,
    SByte = 18,
    Byte = 19,
    // 2 bytes
    Half = 33,
    Int16 = 34,
    UInt16 = 35,
    Char = 36,
    // 4 bytes
    Single = 49,
    Int32 = 50,
    UInt32 = 51,
    PairOfInt16 = 52,
    // 8 bytes
    Double = 65,
    Int64 = 66,
    UInt64 = 67,
    PairOfInt32 = 68,
    // 16 bytes
    Decimal = 81,
    Int128 = 82,
    UInt128 = 83,
    Guid = 84,
    PairOfInt64 = 85,
    QuadOfInt32 = 86,
    RawB10 = 95,
    // 32 bytes
    RawB20 = 111,
    // 64 bytes
    String = 241,
    Binary = 242,
    RawB40 = 127,
    // 128 bytes
    RawB80 = 143,
}

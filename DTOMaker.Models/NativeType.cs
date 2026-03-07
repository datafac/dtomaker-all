using System;

namespace DTOMaker.Models;

public enum NativeType : short
{
    Undefined   = 0x0000,
    // 1-byte types
    SByte       = 0x0001,
    Byte        = 0x0002,
    Boolean     = 0x0003,
    // 2-byte types
    Int16       = 0x0011,
    UInt16      = 0x0012,
    Half        = 0x0013,
    Char        = 0x0014,
    // 4-byte types
    Int32       = 0x0021,
    UInt32      = 0x0022,
    Single      = 0x0023,
    PairOfInt16 = 0x1011,
    // 8-byte types
    Int64       = 0x0031,
    UInt64      = 0x0032,
    Double      = 0x0033,
    PairOfInt32 = 0x1021,
    // 16-byte types
    Int128      = 0x0041,
    UInt128     = 0x0042,
    Decimal     = 0x0043,
    Guid        = 0x0044,
    PairOfInt64 = 0x1031,
    QuadOfInt32 = 0x2021,
    // 32-byte types
    // 64-byte types
    Binary      = 0x0061,
    String      = 0x0062,
}

using DataFac.Memory;
using System;

namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="DataFac.Memory.Bits64"/> properties to be internally 
/// stored and serialized as <see cref="System.UInt64"/> values.
/// </summary>
public sealed class Bits64Converter : IStructConverter<Bits64, UInt64>
{
    public NativeType NativeType => NativeType.UInt64;
    public static Bits64 ToCustom(UInt64 native) => new Bits64(native);
    public static UInt64 ToNative(Bits64 custom) => custom.Data;
    public static Bits64? ToCustom(UInt64? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static UInt64? ToNative(Bits64? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

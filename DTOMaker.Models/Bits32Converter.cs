using DataFac.Memory;
using System;

namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="DataFac.Memory.Bits32"/> properties to be internally 
/// stored and serialized as <see cref="System.UInt32"/> values.
/// </summary>
public sealed class Bits32Converter : IStructConverter<Bits32, UInt32>
{
    public NativeType NativeType => NativeType.UInt32;
    public static Bits32 ToCustom(UInt32 native) => new Bits32(native);
    public static UInt32 ToNative(Bits32 custom) => custom.Data;
    public static Bits32? ToCustom(UInt32? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static UInt32? ToNative(Bits32? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

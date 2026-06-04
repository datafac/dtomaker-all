using System;

namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="DTOMaker.Models.NativeType"/> properties to be internally 
/// stored and serialized as <see cref="System.Int32"/> values.
/// </summary>
public sealed class NativeTypeConverter : IStructConverter<NativeType, Int16>
{
    public NativeType NativeType => NativeType.Int16;
    public static NativeType ToCustom(Int16 native) => (NativeType)native;
    public static Int16 ToNative(NativeType custom) => (Int16)custom;
    public static NativeType? ToCustom(Int16? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static Int16? ToNative(NativeType? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

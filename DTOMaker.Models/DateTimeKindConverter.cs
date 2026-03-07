using System;

namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.DateTimeKind"/> properties to be internally 
/// stored and serialized as <see cref="System.Int32"/> values.
/// </summary>
public sealed class DateTimeKindConverter : IStructConverter<DateTimeKind, byte>
{
    public NativeType NativeType => NativeType.Byte;
    public static DateTimeKind ToCustom(byte native) => (DateTimeKind)native;
    public static byte ToNative(DateTimeKind custom) => (byte)custom;
    public static DateTimeKind? ToCustom(byte? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static byte? ToNative(DateTimeKind? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

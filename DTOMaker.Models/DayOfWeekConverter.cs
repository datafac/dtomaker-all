using System;

namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.DayOfWeek"/> properties to be internally 
/// stored and serialized as <see cref="System.Int32"/> values.
/// </summary>
public sealed class DayOfWeekConverter : IStructConverter<DayOfWeek, byte>
{
    public NativeType NativeType => NativeType.Byte;
    public static DayOfWeek ToCustom(byte native) => (DayOfWeek)native;
    public static byte ToNative(DayOfWeek custom) => (byte)custom;
    public static DayOfWeek? ToCustom(byte? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static byte? ToNative(DayOfWeek? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

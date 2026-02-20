using System;

namespace DTOMaker.Models;

#if NET8_0_OR_GREATER
/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.DateOnly"/> properties to be internally 
/// stored and serialized as <see cref="System.Int32"/> values.
/// </summary>
public sealed class DateOnlyConverter : IStructConverter<DateOnly, int>
{
    public NativeType NativeType => NativeType.Int32;
    public static DateOnly ToCustom(int native) => DateOnly.FromDayNumber(native);
    public static int ToNative(DateOnly custom) => custom.DayNumber;
    public static DateOnly? ToCustom(int? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static int? ToNative(DateOnly? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}
#endif

using System;

namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.DayOfWeek"/> properties to be internally 
/// stored and serialized as <see cref="System.Int32"/> values.
/// </summary>
public sealed class DayOfWeekConverter : IStructConverter<DayOfWeek, int>
{
    public static DayOfWeek ToCustom(int native) => (DayOfWeek)native;
    public static DayOfWeek? ToCustom(int? native) => native.HasValue ? (DayOfWeek)native.Value : null;
    public static int ToNative(DayOfWeek custom) => (int)custom;
    public static int? ToNative(DayOfWeek? custom) => custom.HasValue ? (int)custom.Value : null;
}

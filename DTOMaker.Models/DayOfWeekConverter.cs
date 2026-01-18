using System;

namespace DTOMaker.Models;

public sealed class DayOfWeekConverter : IStructConverter<DayOfWeek, int>
{
    public static DayOfWeek ToCustom(int native) => (DayOfWeek)native;
    public static DayOfWeek? ToCustom(int? native) => native.HasValue ? (DayOfWeek)native.Value : null;
    public static int ToNative(DayOfWeek custom) => (int)custom;
    public static int? ToNative(DayOfWeek? custom) => custom.HasValue ? (int)custom.Value : null;
}

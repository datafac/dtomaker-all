using DataFac.Memory;
using System;

namespace DTOMaker.Models;

public sealed class TimeSpanConverter : IStructConverter<TimeSpan, long>
{
    public static TimeSpan ToCustom(long native) => TimeSpan.FromTicks(native);
    public static TimeSpan? ToCustom(long? native) => native.HasValue ? TimeSpan.FromTicks(native.Value) : null;
    public static long ToNative(TimeSpan custom) => custom.Ticks;
    public static long? ToNative(TimeSpan? custom) => custom.HasValue ? custom.Value.Ticks : null;
}

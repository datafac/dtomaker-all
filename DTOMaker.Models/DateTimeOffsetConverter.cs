using DataFac.Memory;
using System;

namespace DTOMaker.Models;

public sealed class DateTimeOffsetConverter : IStructConverter<DateTimeOffset, PairOfInt64>
{
    public static DateTimeOffset ToCustom(PairOfInt64 native)
    {
        var dt = DateTime.FromBinary(native.A);
        var ts = TimeSpan.FromTicks(native.B);
        return new DateTimeOffset(dt, ts);
    }

    public static DateTimeOffset? ToCustom(PairOfInt64? native)
        => native.HasValue
            ? new DateTimeOffset(DateTime.FromBinary(native.Value.A), TimeSpan.FromTicks(native.Value.B))
            : null;
    public static PairOfInt64 ToNative(DateTimeOffset custom)
    {
        var a = custom.DateTime.ToBinary();
        var b = custom.Offset.Ticks;
        return new PairOfInt64(a, b);
    }

    public static PairOfInt64? ToNative(DateTimeOffset? custom)
        => custom.HasValue
            ? new PairOfInt64(custom.Value.DateTime.ToBinary(), custom.Value.TimeOfDay.Ticks)
            : null;
}


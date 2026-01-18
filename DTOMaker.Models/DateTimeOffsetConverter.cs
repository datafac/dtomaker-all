using DataFac.Memory;
using System;

namespace DTOMaker.Models;

public sealed class DateTimeOffsetConverter : IStructConverter<DateTimeOffset, PairOfInt64>
{
    public static DateTimeOffset ToCustom(PairOfInt64 native)
        => new DateTimeOffset(DateTime.FromBinary(native.A), TimeSpan.FromTicks(native.B));
    public static DateTimeOffset? ToCustom(PairOfInt64? native)
        => native.HasValue
            ? new DateTimeOffset(DateTime.FromBinary(native.Value.A), TimeSpan.FromTicks(native.Value.B))
            : null;
    public static PairOfInt64 ToNative(DateTimeOffset custom)
        => new PairOfInt64(custom.DateTime.ToBinary(), custom.TimeOfDay.Ticks);
    public static PairOfInt64? ToNative(DateTimeOffset? custom)
        => custom.HasValue
            ? new PairOfInt64(custom.Value.DateTime.ToBinary(), custom.Value.TimeOfDay.Ticks)
            : null;
}


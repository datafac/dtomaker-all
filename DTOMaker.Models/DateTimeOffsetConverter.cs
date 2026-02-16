using DataFac.Memory;
using System;

namespace DTOMaker.Models;

public sealed class DateTimeOffsetConverter : IStructConverter<DateTimeOffset, PairOfInt64>
{
    public NativeType NativeType => NativeType.PairOfInt64;

    public static DateTimeOffset ToCustom(PairOfInt64 native)
    {
        var dt = DateTime.FromBinary(native.A);
        var ts = TimeSpan.FromTicks(native.B);
        return new DateTimeOffset(dt, ts);
    }

    public static PairOfInt64 ToNative(DateTimeOffset custom)
    {
        var a = custom.DateTime.ToBinary();
        var b = custom.Offset.Ticks;
        return new PairOfInt64(a, b);
    }

    public static DateTimeOffset? ToCustom(PairOfInt64? native) => native.HasValue ? ToCustom(native.Value): null;
    public static PairOfInt64? ToNative(DateTimeOffset? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}


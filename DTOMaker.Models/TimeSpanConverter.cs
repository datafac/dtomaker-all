using DataFac.Memory;
using System;

namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.TimeSpan"/> properties to be internally 
/// stored and serialized as <see cref="System.Int64"/> values.
/// </summary>
public sealed class TimeSpanConverter : IStructConverter<TimeSpan, long>
{
    public static TimeSpan ToCustom(long native) => TimeSpan.FromTicks(native);
    public static TimeSpan? ToCustom(long? native) => native.HasValue ? TimeSpan.FromTicks(native.Value) : null;
    public static long ToNative(TimeSpan custom) => custom.Ticks;
    public static long? ToNative(TimeSpan? custom) => custom.HasValue ? custom.Value.Ticks : null;
}

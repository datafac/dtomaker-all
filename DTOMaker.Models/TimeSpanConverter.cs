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
    public NativeType NativeType => NativeType.Int64;
    public static TimeSpan ToCustom(long native) => TimeSpan.FromTicks(native);
    public static long ToNative(TimeSpan custom) => custom.Ticks;
    public static TimeSpan? ToCustom(long? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static long? ToNative(TimeSpan? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

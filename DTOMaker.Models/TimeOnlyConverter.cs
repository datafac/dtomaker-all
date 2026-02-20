using System;

namespace DTOMaker.Models;

#if NET8_0_OR_GREATER
/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.TimeOnly"/> properties to be internally 
/// stored and serialized as <see cref="System.Int64"/> values.
/// </summary>
public sealed class TimeOnlyConverter : IStructConverter<TimeOnly, long>
{
    public NativeType NativeType => NativeType.Int64;
    public static TimeOnly ToCustom(long native) => new TimeOnly(native);
    public static long ToNative(TimeOnly custom) => custom.Ticks;
    public static TimeOnly? ToCustom(long? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static long? ToNative(TimeOnly? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}
#endif

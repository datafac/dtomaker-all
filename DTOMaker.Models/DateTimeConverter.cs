using System;

namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.DateTime"/> properties to be internally 
/// stored and serialized as <see cref="System.Int64"/> values.
/// </summary>
public sealed class DateTimeConverter : IStructConverter<DateTime, long>
{
    public NativeType NativeType => NativeType.Int64;
    public static DateTime ToCustom(long native) => DateTime.FromBinary(native);
    public static long ToNative(DateTime custom) => custom.ToBinary();
    public static DateTime? ToCustom(long? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static long? ToNative(DateTime? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}


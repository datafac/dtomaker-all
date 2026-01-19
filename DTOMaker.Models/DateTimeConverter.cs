using System;

namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.DateTime"/> properties to be internally 
/// stored and serialized as <see cref="System.Int64"/> values.
/// </summary>
public sealed class DateTimeConverter : IStructConverter<DateTime, long>
{
    public static DateTime ToCustom(long native) => DateTime.FromBinary(native);
    public static DateTime? ToCustom(long? native) => native.HasValue ? DateTime.FromBinary(native.Value) : null;
    public static long ToNative(DateTime custom) => custom.ToBinary();
    public static long? ToNative(DateTime? custom) => custom.HasValue ? custom.Value.ToBinary() : null;
}


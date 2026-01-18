using System;

namespace DTOMaker.Models;

public sealed class DateTimeConverter : IStructConverter<DateTime, long>
{
    public static DateTime ToCustom(long native) => DateTime.FromBinary(native);
    public static DateTime? ToCustom(long? native) => native.HasValue ? DateTime.FromBinary(native.Value) : null;
    public static long ToNative(DateTime custom) => custom.ToBinary();
    public static long? ToNative(DateTime? custom) => custom.HasValue ? custom.Value.ToBinary() : null;
}


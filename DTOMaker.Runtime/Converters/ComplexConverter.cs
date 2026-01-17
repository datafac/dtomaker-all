using DataFac.Memory;
using System;
using System.Numerics;

namespace DTOMaker.Runtime.Converters;

public sealed class ComplexConverter : IStructConverter<Complex, PairOfInt64>
{
    public static Complex ToCustom(PairOfInt64 native) => new Complex(BitConverter.Int64BitsToDouble(native.A), BitConverter.Int64BitsToDouble(native.B));
    public static Complex? ToCustom(PairOfInt64? native) => native.HasValue ? new Complex(BitConverter.Int64BitsToDouble(native.Value.A), BitConverter.Int64BitsToDouble(native.Value.B)) : null;
    public static PairOfInt64 ToNative(Complex custom) => new PairOfInt64(BitConverter.DoubleToInt64Bits(custom.Real), BitConverter.DoubleToInt64Bits(custom.Imaginary));
    public static PairOfInt64? ToNative(Complex? custom) => custom.HasValue ? new PairOfInt64(BitConverter.DoubleToInt64Bits(custom.Value.Real), BitConverter.DoubleToInt64Bits(custom.Value.Imaginary)) : null;
}

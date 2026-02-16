using DataFac.Memory;
using DTOMaker.Models;
using System;
using System.Numerics;

namespace DTOMaker.Converters.Numerics;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.Numerics.Complex"/> properties to be internally 
/// stored and serialized as <see cref="DataFac.Memory.PairOfInt64"/> values.
/// </summary>
public sealed class ComplexConverter : IStructConverter<Complex, PairOfInt64>
{
    public NativeType NativeType => NativeType.PairOfInt64;
    public static Complex ToCustom(PairOfInt64 native) => new Complex(BitConverter.Int64BitsToDouble(native.A), BitConverter.Int64BitsToDouble(native.B));
    public static PairOfInt64 ToNative(Complex custom) => new PairOfInt64(BitConverter.DoubleToInt64Bits(custom.Real), BitConverter.DoubleToInt64Bits(custom.Imaginary));
    public static Complex? ToCustom(PairOfInt64? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static PairOfInt64? ToNative(Complex? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

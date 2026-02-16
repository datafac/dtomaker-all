using DataFac.Memory;
using DTOMaker.Models;
using System;
using System.Numerics;

namespace DTOMaker.Converters.Numerics;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.Numerics.Vector2"/> properties to be internally 
/// stored and serialized as <see cref="DataFac.Memory.PairOfInt32"/> values.
/// </summary>
public sealed class Vector2Converter : IStructConverter<Vector2, PairOfInt32>
{
    public NativeType NativeType => NativeType.PairOfInt32;
    public static Vector2 ToCustom(PairOfInt32 native)
    {
#if NET8_0_OR_GREATER
        return new Vector2(BitConverter.Int32BitsToSingle(native.A), BitConverter.Int32BitsToSingle(native.B));
#else
        BlockB008 block = default;
        block.PairOfInt32LE = native;
        float x = block.A.SingleValueLE;
        float y = block.B.SingleValueLE;
        return new Vector2(x, y);
#endif
    }

    public static PairOfInt32 ToNative(Vector2 custom)
    {
#if NET8_0_OR_GREATER
        return new PairOfInt32(BitConverter.SingleToInt32Bits(custom.X), BitConverter.SingleToInt32Bits(custom.Y));
#else
        BlockB008 block = default;
        block.A.SingleValueLE = custom.X;
        block.B.SingleValueLE = custom.Y;
        return block.PairOfInt32LE;
#endif
    }

    public static Vector2? ToCustom(PairOfInt32? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static PairOfInt32? ToNative(Vector2? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

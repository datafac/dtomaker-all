using DataFac.Memory;
using DTOMaker.Models;
using System;
using System.Numerics;

namespace DTOMaker.Converters.Numerics;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.Numerics.Vector3"/> properties to be internally 
/// stored and serialized as <see cref="DataFac.Memory.QuadOfInt32"/> values.
/// </summary>
public sealed class Vector3Converter : IStructConverter<Vector3, QuadOfInt32>
{
    public NativeType NativeType => NativeType.QuadOfInt32;
    public static Vector3 ToCustom(QuadOfInt32 native)
    {
#if NET8_0_OR_GREATER
        return new Vector3(BitConverter.Int32BitsToSingle(native.A), BitConverter.Int32BitsToSingle(native.B), BitConverter.Int32BitsToSingle(native.C));
#else
        BlockB016 block = default;
        block.QuadOfInt32LE = native;
        float x = block.A.A.SingleValueLE;
        float y = block.A.B.SingleValueLE;
        float z = block.B.A.SingleValueLE;
        return new Vector3(x, y, z);
#endif
    }

    public static QuadOfInt32 ToNative(Vector3 custom)
    {
#if NET8_0_OR_GREATER
        return new QuadOfInt32(BitConverter.SingleToInt32Bits(custom.X), BitConverter.SingleToInt32Bits(custom.Y), BitConverter.SingleToInt32Bits(custom.Z), 0);
#else
        BlockB016 block = default;
        block.A.A.SingleValueLE = custom.X;
        block.A.B.SingleValueLE = custom.Y;
        block.B.A.SingleValueLE = custom.Z;
        block.B.A.SingleValueLE = 0F;
        return block.QuadOfInt32LE;
#endif
    }

    public static Vector3? ToCustom(QuadOfInt32? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static QuadOfInt32? ToNative(Vector3? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

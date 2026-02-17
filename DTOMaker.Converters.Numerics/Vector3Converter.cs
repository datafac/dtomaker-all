using DataFac.Memory;
using DTOMaker.Models;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

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
        var nativeA = native.A;
        var nativeB = native.B;
        var nativeC = native.C;
        return new Vector3(Unsafe.As<int, float>(ref nativeA), Unsafe.As<int, float>(ref nativeB), Unsafe.As<int, float>(ref nativeC));
    }

    public static QuadOfInt32 ToNative(Vector3 custom)
    {
        var customX = custom.X;
        var customY = custom.Y;
        var customZ = custom.Z;
        return new QuadOfInt32(Unsafe.As<float, int>(ref customX), Unsafe.As<float, int>(ref customY), Unsafe.As<float, int>(ref customZ), 0);
    }

    public static Vector3? ToCustom(QuadOfInt32? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static QuadOfInt32? ToNative(Vector3? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

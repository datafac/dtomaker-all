using DataFac.Memory;
using DTOMaker.Models;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DTOMaker.Converters.Numerics;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.Numerics.Vector4"/> properties to be internally 
/// stored and serialized as <see cref="DataFac.Memory.QuadOfInt32"/> values.
/// </summary>
public sealed class Vector4Converter : IStructConverter<Vector4, QuadOfInt32>
{
    public NativeType NativeType => NativeType.QuadOfInt32;
    public static Vector4 ToCustom(QuadOfInt32 native)
    {
        var nativeA = native.A;
        var nativeB = native.B;
        var nativeC = native.C;
        var nativeD = native.D;
        return new Vector4(Unsafe.As<int, float>(ref nativeA), Unsafe.As<int, float>(ref nativeB), Unsafe.As<int, float>(ref nativeC), Unsafe.As<int, float>(ref nativeD));
    }

    public static QuadOfInt32 ToNative(Vector4 custom)
    {
        var customX = custom.X;
        var customY = custom.Y;
        var customZ = custom.Z;
        var customD = custom.W;
        return new QuadOfInt32(Unsafe.As<float, int>(ref customX), Unsafe.As<float, int>(ref customY), Unsafe.As<float, int>(ref customZ), Unsafe.As<float, int>(ref customD));
    }

    public static Vector4? ToCustom(QuadOfInt32? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static QuadOfInt32? ToNative(Vector4? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

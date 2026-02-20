using DataFac.Memory;
using DTOMaker.Models;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DTOMaker.Converters.Numerics;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.Numerics.Quaternion"/> properties to be internally 
/// stored and serialized as <see cref="DataFac.Memory.QuadOfInt32"/> values.
/// </summary>
public sealed class QuaternionConverter : IStructConverter<Quaternion, QuadOfInt32>
{
    public NativeType NativeType => NativeType.QuadOfInt32;
    public static Quaternion ToCustom(QuadOfInt32 native)
    {
        var nativeA = native.A;
        var nativeB = native.B;
        var nativeC = native.C;
        var nativeD = native.D;
        return new Quaternion(Unsafe.As<int, float>(ref nativeA), Unsafe.As<int, float>(ref nativeB), Unsafe.As<int, float>(ref nativeC), Unsafe.As<int, float>(ref nativeD));
    }

    public static QuadOfInt32 ToNative(Quaternion custom)
    {
        var customX = custom.X;
        var customY = custom.Y;
        var customZ = custom.Z;
        var customW = custom.W;
        return new QuadOfInt32(Unsafe.As<float, int>(ref customX), Unsafe.As<float, int>(ref customY), Unsafe.As<float, int>(ref customZ), Unsafe.As<float, int>(ref customW));
    }

    public static Quaternion? ToCustom(QuadOfInt32? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static QuadOfInt32? ToNative(Quaternion? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

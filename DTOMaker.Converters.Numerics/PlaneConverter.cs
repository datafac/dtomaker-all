using DataFac.Memory;
using DTOMaker.Models;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DTOMaker.Converters.Numerics;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.Numerics.Plane"/> properties to be internally 
/// stored and serialized as <see cref="DataFac.Memory.QuadOfInt32"/> values.
/// </summary>
public sealed class PlaneConverter : IStructConverter<Plane, QuadOfInt32>
{
    public NativeType NativeType => NativeType.QuadOfInt32;
    public static Plane ToCustom(QuadOfInt32 native)
    {
        var nativeA = native.A;
        var nativeB = native.B;
        var nativeC = native.C;
        var nativeD = native.D;
        return new Plane(Unsafe.As<int, float>(ref nativeA), Unsafe.As<int, float>(ref nativeB), Unsafe.As<int, float>(ref nativeC), Unsafe.As<int, float>(ref nativeD));
    }

    public static QuadOfInt32 ToNative(Plane custom)
    {
        var customX = custom.Normal.X;
        var customY = custom.Normal.Y;
        var customZ = custom.Normal.Z;
        var customD = custom.D;
        return new QuadOfInt32(Unsafe.As<float, int>(ref customX), Unsafe.As<float, int>(ref customY), Unsafe.As<float, int>(ref customZ), Unsafe.As<float, int>(ref customD));
    }

    public static Plane? ToCustom(QuadOfInt32? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static QuadOfInt32? ToNative(Plane? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

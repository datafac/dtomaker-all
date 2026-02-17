using DataFac.Memory;
using DTOMaker.Models;
using System.Numerics;
using System.Runtime.CompilerServices;

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
        var nativeA = native.A;
        var nativeB = native.B;
        return new Vector2(Unsafe.As<int, float>(ref nativeA), Unsafe.As<int, float>(ref nativeB));
    }

    public static PairOfInt32 ToNative(Vector2 custom)
    {
        var customX = custom.X;
        var customY = custom.Y;
        return new PairOfInt32(Unsafe.As<float, int>(ref customX), Unsafe.As<float, int>(ref customY));
    }

    public static Vector2? ToCustom(PairOfInt32? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static PairOfInt32? ToNative(Vector2? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="DTOMaker.Models.NativeType"/> properties to be internally 
/// stored and serialized as <see cref="System.Int32"/> values.
/// </summary>
public sealed class NativeTypeConverter : IStructConverter<NativeType, short>
{
    public NativeType NativeType => NativeType.Int32;
    public static NativeType ToCustom(short native) => (NativeType)native;
    public static short ToNative(NativeType custom) => (short)custom;
    public static NativeType? ToCustom(short? native) => native.HasValue ? ToCustom(native.Value) : null;
    public static short? ToNative(NativeType? custom) => custom.HasValue ? ToNative(custom.Value) : null;
}

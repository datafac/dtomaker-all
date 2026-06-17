namespace DTOMaker.Models;

/// <summary>
/// Represents a value type converter that can be used to convert between a user-defined 
/// struct type and a built-in struct type for internal storage and serialization purposes.
/// Note that value type conversions require no heap allocations, so are efficient. For 
/// reference type conversions, see <see cref="IObjectConverter{TCustom, TNative}"/>.
/// </summary>
/// <typeparam name="TCustom"></typeparam>
/// <typeparam name="TNative"></typeparam>
public interface IStructConverter<TCustom, TNative>
    where TCustom : struct
    where TNative : struct
{
    NativeType NativeType { get; }
#if NET8_0_OR_GREATER
    static abstract TNative ToNative(TCustom custom);
    static abstract TNative? ToNative(TCustom? custom);
    static abstract TCustom ToCustom(TNative native);
    static abstract TCustom? ToCustom(TNative? native);
#endif
}


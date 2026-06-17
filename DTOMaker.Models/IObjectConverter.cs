namespace DTOMaker.Models;

/// <summary>
/// Represents a reference type converter that can be used to convert between a user-defined 
/// reference type and a built-in reference type (string or binary) for internal storage 
/// and serialization purposes. Note that reference type conversions usually involve heap 
/// allocations, so may be less efficient than value type conversions. For value type conversions, 
/// see <see cref="IStructConverter{TCustom, TNative}"/>.
/// </summary>
/// <typeparam name="TCustom"></typeparam>
/// <typeparam name="TNative"></typeparam>
public interface IObjectConverter<TCustom, TNative>
    where TCustom : class
    where TNative : class
{
    NativeType NativeType { get; }
#if NET8_0_OR_GREATER
    static abstract TNative? ToNative(TCustom? custom);
    static abstract TCustom? ToCustom(TNative? native);
#endif
}


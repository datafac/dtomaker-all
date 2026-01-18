using DataFac.Memory;
namespace DTOMaker.Models;

public interface IStructConverter<TCustom, TNative>
    where TCustom : struct
    where TNative : struct
{
#if NET8_0_OR_GREATER
    static abstract TNative ToNative(TCustom custom);
    static abstract TNative? ToNative(TCustom? custom);
    static abstract TCustom ToCustom(TNative native);
    static abstract TCustom? ToCustom(TNative? native);
#endif
}

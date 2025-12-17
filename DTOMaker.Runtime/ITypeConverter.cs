namespace DTOMaker.Runtime
{
    public interface ITypeConverter<TCustom, TNative>
    {
        TNative ToNative(TCustom custom);
        TCustom ToCustom(TNative native);
    }
}

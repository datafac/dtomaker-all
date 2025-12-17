namespace DTOMaker.SrcGen.Core
{
    public interface ILanguage
    {
        string CommentPrefix { get; }
        string CommandPrefix { get; }
        string TokenPrefix { get; }
        string TokenSuffix { get; }
        string GetDataTypeToken(TypeFullName typeFullName);
        string GetDefaultValue(TypeFullName typeFullName);
        string GetValueAsCode(object? value);
    }
}
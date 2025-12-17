namespace DTOMaker.CLI
{
    internal interface ITargetLanguage
    {
        string Name { get; }
        string PrefixComment { get; }
        string PrefixMetaCode { get; }
        string EmitCodePrefix { get; }
        string EmitCodeSuffix { get; }
    }
}

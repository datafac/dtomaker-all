namespace DTOMaker.SrcGen.Core;

public sealed record class SourceGeneratorParameters
{
    /// <summary>
    /// Gets the language configuration used for processing or interpreting content.
    /// </summary>
    public ILanguage Language { get; init; } = Language_CSharp.Instance;

    /// <summary>
    /// The suffix appended to implementation namespaces. This is a
    /// constant defined by each source generator template and runtime, and 
    /// cannot be changed easily.
    /// </summary>
    public string ImplSpaceSuffix { get; init; } = "Generated";
}

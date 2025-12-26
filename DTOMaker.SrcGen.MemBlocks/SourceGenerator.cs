using DTOMaker.SrcGen.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace DTOMaker.SrcGen.MemBlocks
{

    [Generator]
    public sealed class SourceGenerator : SourceGeneratorBase
    {
        private static readonly SourceGeneratorParameters _parameters = new SourceGeneratorParameters()
        {
            GeneratorId = GeneratorId.MemBlocks,
            Language = Language_CSharp.Instance,
            ImplSpaceSuffix = "MemBlocks"
        };
        protected override SourceGeneratorParameters OnBeginInitialize(IncrementalGeneratorInitializationContext context) => _parameters;
        protected override void OnEndInitialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<OutputEntity> entities)
        {
            // generate entities
            context.RegisterSourceOutput(entities, (spc, ent) =>
            {
                var generator = new EntityGenerator(_parameters);
                string source = generator.GenerateSourceText(ent);
                string hintName = $"{ent.TFN.Impl.Space}.{ent.TFN.Impl.Name}.g.cs";
                spc.AddSource(hintName, SourceText.From(source, Encoding.UTF8));
            });
        }
    }
}

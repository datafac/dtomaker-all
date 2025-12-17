using DTOMaker.SrcGen.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace DTOMaker.SrcGen.JsonSystemText
{

    [Generator]
    public sealed class JsonSTSourceGenerator : SourceGeneratorBase
    {
        protected override void OnBeginInitialize(IncrementalGeneratorInitializationContext context)
        {
        }
        protected override void OnEndInitialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<OutputEntity> entities)
        {
            // generate entities
            context.RegisterSourceOutput(entities, (spc, ent) =>
            {
                var generator = new EntityGenerator(Language_CSharp.Instance);
                string source = generator.GenerateSourceText(ent);
                string hintName = $"{ent.Impl.Space}.{ent.Impl.Name}.g.cs";
                spc.AddSource(hintName, SourceText.From(source, Encoding.UTF8));
            });
        }
    }
}

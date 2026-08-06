using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using System;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace DTOMaker.TestHelpers
{
    public static class GeneratorTestHelper
    {
        private static Compilation CreateCompilation(string source,
            LanguageVersion languageVersion,
            params PortableExecutableReference[] additionalReferences)
        {
            Assembly standardAssm = Assembly.Load("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51");
#if NET8_0_OR_GREATER
            Assembly runtimeAssm = Assembly.Load("System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
#endif
            PortableExecutableReference[] commonReferences = new[]
                        {
                            MetadataReference.CreateFromFile(standardAssm.Location),
#if NET8_0_OR_GREATER
                            MetadataReference.CreateFromFile(runtimeAssm.Location),
#endif
                            MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                            //MetadataReference.CreateFromFile(typeof(Attribute).GetTypeInfo().Assembly.Location),
                            MetadataReference.CreateFromFile(typeof(Memory<>).GetTypeInfo().Assembly.Location),
                            MetadataReference.CreateFromFile(typeof(Complex).GetTypeInfo().Assembly.Location),
                            // types for local tests
                            MetadataReference.CreateFromFile(typeof(DataFac.Memory.Octets).GetTypeInfo().Assembly.Location),
                            MetadataReference.CreateFromFile(typeof(DTOMaker.Models.EntityAttribute).GetTypeInfo().Assembly.Location),
                        };

            PortableExecutableReference[] metadataReferences = commonReferences.Concat(additionalReferences).ToArray();

            // how to use LanguageVersion.Preview
            // https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
            //

            var compilation = CSharpCompilation.Create("compilation",
                        new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(languageVersion)) },
                        metadataReferences,
                        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            return compilation;
        }

        private static GeneratorRunResult RunSourceGenerator(
            IIncrementalGenerator generator,
            string source,
            LanguageVersion languageVersion,
            params PortableExecutableReference[] additionalReferences)
        {
            // Create the 'input' compilation that the generator will act on
            Compilation inputCompilation = CreateCompilation(source, languageVersion, additionalReferences);

            // directly create an instance of the generator
            // (Note: in the compiler this is loaded from an assembly, and created via reflection at runtime)
            //ISourceGenerator generator = new SourceGenerator();

            // Create the driver that will control the generation, passing in our generator
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the generation pass
            // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
            //driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            driver = driver.RunGenerators(inputCompilation);

            // We can now assert things about the resulting compilation:
            //diagnostics.ShouldBeEmpty(); // there were no diagnostics created by the generators
            //outputCompilation.SyntaxTrees.Count.ShouldBe(1 + expectedNewTrees); // we have two syntax trees, the original 'user' provided one, and the one added by the generator
            //outputCompilation.GetDiagnostics().ShouldBeEmpty(); // verify the compilation with the added source has no diagnostics

            // Or we can look at the results directly:
            GeneratorDriverRunResult runResult = driver.GetRunResult();

            // The runResult contains the combined results of all generators passed to the driver
            //runResult.GeneratedTrees.Length.ShouldBe(expectedNewTrees);
            //runResult.Diagnostics.ShouldBeEmpty();

            var generatorResult = runResult.Results[0];

            return generatorResult;
        }

        public static string GenerateAndGetOutput(this IIncrementalGenerator generator, string modelSource, int index, string expectedHintName)
        {
            VerifyTestHelpers.EnsureInitialized();
            var generatorResult = GeneratorTestHelper.RunSourceGenerator(generator, modelSource, LanguageVersion.LatestMajor);
            var generated = generatorResult.GeneratedSources[index];
            generated.HintName.ShouldBe(expectedHintName);
            string outputCode = string.Join(Environment.NewLine, generated.SourceText.Lines.Select(tl => tl.ToString()));
            return outputCode;
        }

        public static void GenerateAndCheckLength(this IIncrementalGenerator generator, string modelSource, int expectedLength, string? expectedErrorCodes = null)
        {
            VerifyTestHelpers.EnsureInitialized();
            var generatorResult = GeneratorTestHelper.RunSourceGenerator(generator, modelSource, LanguageVersion.LatestMajor);
            generatorResult.Exception.ShouldBeNull();
            generatorResult.Diagnostics.Count(d => d.Id == "INF01").ShouldBe(1);
            generatorResult.Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Info).ShouldBe(1);
            generatorResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning).ShouldBeEmpty();

            int expectedErrorCount = 0;
            if (expectedErrorCodes is null)
            {
                expectedErrorCount = 0;
                generatorResult.Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error).ShouldBe(expectedErrorCount);
            }
            else
            {
                string[] codes = expectedErrorCodes.Split(',');
                expectedErrorCount = codes.Length;
                generatorResult.Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error).ShouldBe(expectedErrorCount);
                foreach (var code in codes)
                {
                    int expectedCodeCount = codes.Count(c => c == code);
                    generatorResult.Diagnostics.Count(d => d.Id == code).ShouldBe(expectedCodeCount);
                }
            }

            // custom generation checks
            generatorResult.GeneratedSources.Length.ShouldBe(expectedLength);
        }

    }
}

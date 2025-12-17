//using Microsoft.CodeAnalysis.CSharp;
//using Shouldly;
//using System.Threading.Tasks;
//using VerifyXunit;
//using Xunit;

//namespace DTOMaker.SrcGen.JsonSystemText.Tests
//{
//    public class GenericsTests
//    {
//        private readonly string source1 =
//            """
//            using System;
//            using DTOMaker.Models;
//            using DTOMaker.Runtime;
//            namespace MyOrg.Models
//            {
//                [Entity][Id(1)]
//                public interface IPair<T1, T2> : IEntityBase
//                {
//                    [Member(1)] T1 Item1 {get;set;}
//                    [Member(2)] T2 Item2 {get;set;}
//                }
//                [Entity][Id(2)]
//                public interface IMyDTO2 : IPair<long, string>
//                {
//                    [Member(1)] int Id {get;set;}
//                }
//                [Entity][Id(3)]
//                public interface IMyDTO3 : IEntityBase
//                {
//                    [Member(1)] int Id {get;set;}
//                    [Member(2)] IPair<long, string>? Pair {get;set;}
//                }
//            }
//            """;

//        [Fact] public void Generic1_GeneratedSourcesLength() => source1.GenerateAndCheckLength(4);
//        [Fact] public async Task Generic1_VerifyGeneratedSource0() => await Verifier.Verify(source1.GenerateAndGetOutput(0, "DTOMaker.Runtime.JsonSystemText.EntityBase.g.cs"));
//        [Fact] public async Task Generic1_VerifyGeneratedSource1() => await Verifier.Verify(source1.GenerateAndGetOutput(1, "MyOrg.Models.JsonSystemText.MyDTO2.g.cs"));
//        [Fact] public async Task Generic1_VerifyGeneratedSource2() => await Verifier.Verify(source1.GenerateAndGetOutput(2, "MyOrg.Models.JsonSystemText.MyDTO3.g.cs"));
//        [Fact] public async Task Generic1_VerifyGeneratedSource3() => await Verifier.Verify(source1.GenerateAndGetOutput(3, "MyOrg.Models.JsonSystemText.Pair_2_Int64_String.g.cs"));

//        private readonly string source2 =
//            """
//            using System;
//            using DataFac.Memory;
//            using DTOMaker.Models;
//            namespace MyOrg.Models
//            {
//                [Entity][Id(1)]
//                public interface IMonoid<T1>
//                {
//                    [Member(1)] T1 Value {get;set;}
//                }
//                [Entity][Id(2)]
//                public interface IMyDTO : IMonoid<_T1_>
//                {
//                }
//            }
//            """;

//        [Fact]
//        public void Generic2_Monoid1_CheckGeneratedSources()
//        {
//            string source = source2.Replace("_T1_", "bool");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Exception.ShouldBeNull();
//            grr.Diagnostics.ShouldBeEmpty();

//            grr.GeneratedSources.Length.ShouldBe(2);
//            grr.GeneratedSources[0].HintName.ShouldBe("MyOrg.Models.Monoid_1_Boolean.JsonSystemText.g.cs");
//            grr.GeneratedSources[1].HintName.ShouldBe("MyOrg.Models.MyDTO.JsonSystemText.g.cs");
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_bool()
//        {
//            string source = source2.Replace("_T1_", "bool");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_byte()
//        {
//            string source = source2.Replace("_T1_", "byte");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_sbyte()
//        {
//            string source = source2.Replace("_T1_", "sbyte");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_short()
//        {
//            string source = source2.Replace("_T1_", "short");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_ushort()
//        {
//            string source = source2.Replace("_T1_", "ushort");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_char()
//        {
//            string source = source2.Replace("_T1_", "char");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_int()
//        {
//            string source = source2.Replace("_T1_", "int");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_uint()
//        {
//            string source = source2.Replace("_T1_", "uint");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_float()
//        {
//            string source = source2.Replace("_T1_", "float");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_long()
//        {
//            string source = source2.Replace("_T1_", "long");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_ulong()
//        {
//            string source = source2.Replace("_T1_", "ulong");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_double()
//        {
//            string source = source2.Replace("_T1_", "double");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_string()
//        {
//            string source = source2.Replace("_T1_", "string");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_Guid()
//        {
//            string source = source2.Replace("_T1_", "Guid");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_decimal()
//        {
//            string source = source2.Replace("_T1_", "decimal");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_Octets()
//        {
//            string source = source2.Replace("_T1_", "Octets");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//#if NET8_0_OR_GREATER
//        [Fact]
//        public async Task Generic2_Monoid2_Half()
//        {
//            string source = source2.Replace("_T1_", "Half");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_Int128()
//        {
//            string source = source2.Replace("_T1_", "Int128");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic2_Monoid2_UInt128()
//        {
//            string source = source2.Replace("_T1_", "UInt128");
//            var grr = GeneratorTestHelper.RunSourceGenerator(source, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }
//#endif

//        private readonly string source3 =
//            """
//            using System;
//            using DataFac.Memory;
//            using DTOMaker.Models;
//            namespace MyOrg.Models
//            {
//                [Entity][Id(1)]
//                public interface IMonoid<T1>
//                {
//                    [Member(1)] T1? Value {get;set;}
//                }
//                [Entity][Id(2)]
//                public interface IMyDTO : IMonoid<IOther>
//                {
//                }
//                [Entity][Id(3)]
//                public interface IOther
//                {
//                }
//            }
//            """;

//        [Fact]
//        public void Generic3_Monoid1_CheckGeneratedSources()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source3, LanguageVersion.LatestMajor);
//            grr.Exception.ShouldBeNull();
//            grr.Diagnostics.ShouldBeEmpty();

//            grr.GeneratedSources.Length.ShouldBe(3);
//            grr.GeneratedSources[0].HintName.ShouldBe("MyOrg.Models.Monoid_1_Other.JsonSystemText.g.cs");
//            grr.GeneratedSources[1].HintName.ShouldBe("MyOrg.Models.MyDTO.JsonSystemText.g.cs");
//            grr.GeneratedSources[2].HintName.ShouldBe("MyOrg.Models.Other.JsonSystemText.g.cs");
//        }

//        [Fact]
//        public async Task Generic3_Monoid2_Monoid_1_Other()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source3, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic3_Monoid3_MyDTO()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source3, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[1].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic3_Monoid4_Other()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source3, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[2].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        private readonly string source4 =
//            """
//            using System;
//            using DataFac.Memory;
//            using DTOMaker.Models;
//            namespace MyOrg.Models
//            {
//                [Entity][Id(1)]
//                public interface ITree<TK, TV>
//                {
//                    [Member(1)] int Count {get;set;}
//                    [Member(2)] TK  Key   {get;set;}
//                    [Member(3)] TV  Value {get;set;}
//                    [Member(4)] ITree<TK, TV>? Left  {get;set;}
//                    [Member(5)] ITree<TK, TV>? Right {get;set;}
//                }
//                [Entity][Id(2)]
//                public interface IMyTree : ITree<String, Octets>
//                {
//                }
//            }
//            """;

//        [Fact]
//        public void Generic4_Recurse1_CheckGeneratedSources()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source4, LanguageVersion.LatestMajor);
//            grr.Exception.ShouldBeNull();
//            grr.Diagnostics.ShouldBeEmpty();

//            grr.GeneratedSources.Length.ShouldBe(2);
//            grr.GeneratedSources[0].HintName.ShouldBe("MyOrg.Models.MyTree.JsonSystemText.g.cs");
//            grr.GeneratedSources[1].HintName.ShouldBe("MyOrg.Models.Tree_2_String_Octets.JsonSystemText.g.cs");
//        }

//        [Fact]
//        public async Task Generic4_Recurse2_VeryifyMyTree()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source4, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic4_Recurse3_VeryifyTree_2()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source4, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[1].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        private readonly string source5 =
//            """
//            using System;
//            using DataFac.Memory;
//            using DTOMaker.Models;
//            namespace MyOrg.Models
//            {
//                [Entity][Id(100)]
//                public interface IBase<T>
//                {
//                    [Member(1)] T Value {get; set;}
//                }
//                [Entity][Id(101)]
//                public interface IPoco<TK, TV> : IBase<TV>
//                {
//                    [Member(1)] TK Key {get; set;}
//                }
//                [Entity][Id(1)]
//                public interface IMyPoco1 : IPoco<String, Octets>
//                {
//                }
//                [Entity][Id(2)]
//                public interface IMyPoco2
//                {
//                    [Member(1)] IPoco<String, Int64>? Field1 {get; set;}
//                }
//            }
//            """;

//        [Fact]
//        public void Generic5_Nested1_CheckGeneratedSources()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source5, LanguageVersion.LatestMajor);
//            grr.Exception.ShouldBeNull();
//            grr.Diagnostics.ShouldBeEmpty();

//            grr.GeneratedSources.Length.ShouldBe(6);
//            grr.GeneratedSources[0].HintName.ShouldBe("MyOrg.Models.Base_1_Int64.JsonSystemText.g.cs");
//            grr.GeneratedSources[1].HintName.ShouldBe("MyOrg.Models.Base_1_Octets.JsonSystemText.g.cs");
//            grr.GeneratedSources[2].HintName.ShouldBe("MyOrg.Models.MyPoco1.JsonSystemText.g.cs");
//            grr.GeneratedSources[3].HintName.ShouldBe("MyOrg.Models.MyPoco2.JsonSystemText.g.cs");
//            grr.GeneratedSources[4].HintName.ShouldBe("MyOrg.Models.Poco_2_String_Int64.JsonSystemText.g.cs");
//            grr.GeneratedSources[5].HintName.ShouldBe("MyOrg.Models.Poco_2_String_Octets.JsonSystemText.g.cs");
//        }

//        [Fact]
//        public async Task Generic5_Nested1_Verify0_Base_1_Int64()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source5, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[0].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic5_Nested1_Verify1_Base_1_Octets()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source5, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[1].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic5_Nested1_Verify2_MyPoco1()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source5, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[2].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic5_Nested1_Verify3_MyPoco2()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source5, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[3].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic5_Nested1_Verify4_Poco_2_String_Int64()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source5, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[4].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//        [Fact]
//        public async Task Generic5_Nested1_Verify5_Poco_2_String_Octets()
//        {
//            var grr = GeneratorTestHelper.RunSourceGenerator(source5, LanguageVersion.LatestMajor);
//            grr.Diagnostics.ShouldBeEmpty();
//            string outputCode = grr.GeneratedSources[5].SourceText.ToString();
//            await Verifier.Verify(outputCode);
//        }

//    }
//}
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonNewtonSoft.Tests
{
    public class ConverterTests_Enum
    {
        private static readonly string modelSource =
            """
            using System;
            using DataFac.Memory;
            using DTOMaker.Models;
            using DTOMaker.Runtime;
            namespace MyOrg.Models
            {
                [Entity(1)]
                public interface IMyDTO : IEntityBase
                {
                    [Member(1, NativeType.Int32, "DTOMaker.Runtime.Converters.DayOfWeekConverter")] DayOfWeek Field1 { get; set; }
                    [Member(2, NativeType.Int32, "DTOMaker.Runtime.Converters.DayOfWeekConverter")] DayOfWeek? Field2 { get; set; }
                }
            }
            """;

        [Fact] public void CustomSrcGen_GeneratedSourcesLength() => modelSource.GenerateAndCheckLength(1);
        [Fact] public async Task CustomSrcGen_VerifyGeneratedSource0() => await Verifier.Verify(modelSource.GenerateAndGetOutput(0, "MyOrg.Models.JsonNewtonSoft.MyDTO.g.cs"));
    }
}
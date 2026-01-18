using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.Tests
{
    public class ConverterTests_Enum
    {
        private static readonly string modelSource =
            """
            using System;
            using DataFac.Memory;
            using DTOMaker.Models;
            using DTOMaker.Runtime;
            using DTOMaker.Runtime.Converters;
            namespace MyOrg.Models
            {
                [Entity(1)]
                public interface IMyDTO : IEntityBase
                {
                    [Member(1, NativeType.Int32, typeof(DayOfWeekConverter))] DayOfWeek Field1 { get; set; }
                    [Member(2, NativeType.Int32, typeof(DayOfWeekConverter))] DayOfWeek? Field2 { get; set; }
                }
            }
            """;

        [Fact] public void CustomSrcGen_GeneratedSourcesLength() => modelSource.GenerateAndCheckLength(1);
        [Fact] public async Task CustomSrcGen_VerifyGeneratedSource0() => await Verifier.Verify(modelSource.GenerateAndGetOutput(0, "MyOrg.Models.JsonSystemText.MyDTO.g.cs"));
    }
}

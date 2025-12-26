using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.Tests
{
    public class DiagnosticsTests
    {
        [Fact]
        public void InvalidEntityId()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                using DTOMaker.Runtime;
                namespace MyOrg.Models
                {
                    [Entity]
                    public interface IMyDTO : IEntityBase
                    {
                    }
                }
                """;

            modelSource.GenerateAndCheckLength(1, "DME03");
        }

        [Fact]
        public void InvalidMemberId()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                using DTOMaker.Runtime;
                namespace MyOrg.Models
                {
                    [Entity][Id(1)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(0)] int  Field1 { get; set; }
                    }
                }
                """;

            modelSource.GenerateAndCheckLength(1, "DME04");
        }
    }
}

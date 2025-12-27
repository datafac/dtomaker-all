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
                    [Entity(0, LayoutMethod.Linear)]
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
                    [Entity(1, LayoutMethod.Linear)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(0)] int  Field1 { get; set; }
                    }
                }
                """;

            modelSource.GenerateAndCheckLength(1, "DME04");
        }

        [Fact]
        public void InvalidEntityLength()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                using DTOMaker.Runtime;
                namespace MyOrg.Models
                {
                    [Entity(1, LayoutMethod.Linear)] [Length(8000)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(1)] string  Field1 { get; set; }
                    }
                }
                """;

            modelSource.GenerateAndCheckLength(1, "DME05");
        }

        [Fact]
        public void InvalidMemberLength()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                using DTOMaker.Runtime;
                namespace MyOrg.Models
                {
                    [Entity(1, LayoutMethod.Linear)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(1)][Length(15)] string  Field1 { get; set; }
                    }
                }
                """;

            modelSource.GenerateAndCheckLength(1, "DME06");
        }

        [Fact]
        public void InvalidNullableMember()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                using DTOMaker.Runtime;
                namespace MyOrg.Models
                {
                    [Entity(1, LayoutMethod.Linear)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(1)] int? Field1 { get; set; }
                    }
                }
                """;

            modelSource.GenerateAndCheckLength(1, "DME08");
        }

        [Fact]
        public void InvalidMemberType()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                using DTOMaker.Runtime;
                namespace MyOrg.Models
                {
                    [Entity(1, LayoutMethod.Linear)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(1)] DayOfWeek Field1 { get; set; }
                    }
                }
                """;

            modelSource.GenerateAndCheckLength(1, "DME06,DME10");
        }
    }
}

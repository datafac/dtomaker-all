using DTOMaker.TestHelpers;
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
                namespace MyOrg.Models
                {
                    [Entity(0)]
                    public interface IMyDTO : IEntityBase
                    {
                    }
                }
                """;

            new SourceGenerator().GenerateAndCheckLength(modelSource, 3, "DME03");
        }

        [Fact]
        public void DuplicateEntityId()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                namespace MyOrg.Models
                {
                    [Entity(1)] public interface IMyDTO1 : IEntityBase { }
                    [Entity(1)] public interface IMyDTO2 : IEntityBase { }
                }
                """;

            new SourceGenerator().GenerateAndCheckLength(modelSource, 4, "DME12,DME12");
        }

        [Fact]
        public void MissingBaseEntity()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                namespace MyOrg.Models
                {
                    [Entity(1)] public interface IMyDTO1 { }
                }
                """;

            new SourceGenerator().GenerateAndCheckLength(modelSource, 3, "DME14");
        }

        [Fact]
        public void MissingEntityTag()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                namespace MyOrg.Models
                {
                    public interface IMyDTO1 : IEntityBase { }

                    [Entity(2)]
                    public interface IMyDTO2 : IMyDTO1 { }
                }
                """;

            new SourceGenerator().GenerateAndCheckLength(modelSource, 3, "DME14");
        }

        [Fact]
        public void InvalidMemberId()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                namespace MyOrg.Models
                {
                    [Entity(1)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(0)] int  Field1 { get; set; }
                    }
                }
                """;

            new SourceGenerator().GenerateAndCheckLength(modelSource, 3, "DME04,DME11");
        }

        [Fact]
        public void InvalidMemberType()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                namespace MyOrg.Models
                {
                    [Entity(1)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(1)] DayOfWeek Field1 { get; set; }
                    }
                }
                """;

                new SourceGenerator().GenerateAndCheckLength(modelSource, 3, "DME06,DME10");
        }

        [Fact]
        public void ValidCustomMemberType()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                namespace MyOrg.Models
                {
                    [Entity(1)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(1, NativeType.Byte, typeof(DayOfWeekConverter))] DayOfWeek Field1 { get; set; }
                    }
                }
                """;

            new SourceGenerator().GenerateAndCheckLength(modelSource, 3, null);
        }

        [Fact]
        public void InvalidMemberSequence()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                namespace MyOrg.Models
                {
                    [Entity(1)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(1)] int  Field1 { get; set; }
                        [Member(3)] int  Field3 { get; set; }
                    }
                }
                """;

            new SourceGenerator().GenerateAndCheckLength(modelSource, 3, "DME11");
        }

    }
}

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

            modelSource.GenerateAndCheckLength(1, "DME03");
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

            modelSource.GenerateAndCheckLength(2, "DME12,DME12");
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

            modelSource.GenerateAndCheckLength(1, "DME14");
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

            modelSource.GenerateAndCheckLength(1, "DME14");
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

            modelSource.GenerateAndCheckLength(1, "DME04,DME11");
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

            modelSource.GenerateAndCheckLength(1, "DME06,DME10");
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

            modelSource.GenerateAndCheckLength(1, null);
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

            modelSource.GenerateAndCheckLength(1, "DME11");
        }

    }
}

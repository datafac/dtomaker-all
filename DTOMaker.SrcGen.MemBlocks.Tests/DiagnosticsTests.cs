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
        public void DuplicateEntityId()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                using DTOMaker.Runtime;
                namespace MyOrg.Models
                {
                    [Entity(1, LayoutMethod.Linear)] public interface IMyDTO1 : IEntityBase { }
                    [Entity(1, LayoutMethod.Linear)] public interface IMyDTO2 : IEntityBase { }
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
                using DTOMaker.Runtime;
                namespace MyOrg.Models
                {
                    [Entity(1, LayoutMethod.Linear)] public interface IMyDTO1 { }
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
                using DTOMaker.Runtime;
                namespace MyOrg.Models
                {
                    public interface IMyDTO1 : IEntityBase { }

                    [Entity(2, LayoutMethod.Linear)]
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

            modelSource.GenerateAndCheckLength(1, "DME04,DME11");
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

        [Fact]
        public void InvalidMemberSequence()
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
                        [Member(1)] int  Field1 { get; set; }
                        [Member(3)] int  Field3 { get; set; }
                    }
                }
                """;

            modelSource.GenerateAndCheckLength(1, "DME11");
        }

        [Fact]
        public void InvalidMemberLayout()
        {
            string modelSource =
                """
                using System;
                using DataFac.Memory;
                using DTOMaker.Models;
                using DTOMaker.Runtime;
                namespace MyOrg.Models
                {
                    [Entity(1, LayoutMethod.Explicit)]
                    public interface IMyDTO : IEntityBase
                    {
                        [Member(1)] Octets Field1 { get; set; }
                        [Member(2)] Octets Field2 { get; set; }
                    }
                }
                """;

            modelSource.GenerateAndCheckLength(1, "DME13");
        }

    }
}

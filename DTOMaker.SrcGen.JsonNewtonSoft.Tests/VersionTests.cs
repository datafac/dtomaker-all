using Shouldly;
using System;
using Xunit;

namespace DTOMaker.SrcGen.JsonNewtonSoft.Tests
{
    public class VersionTests
    {
        //[Fact]
        //public void ModelsVersionChecks()
        //{
        //    Version coreVersion = typeof(DTOMaker.Models.EntityAttribute).Assembly.GetName().Version ?? new Version(99, 99, 9999);
        //    Version thisVersion = typeof(DTOMaker.Models.JsonNewtonSoft.xxxxxxAttribute).Assembly.GetName().Version ?? new Version(0, 0, 0);

        //    thisVersion.Major.ShouldBe(coreVersion.Major);
        //    thisVersion.Minor.ShouldBeGreaterThanOrEqualTo(coreVersion.Minor);
        //}

        [Fact]
        public void SrcGenVersionChecks()
        {
            Version coreVersion = typeof(DTOMaker.SrcGen.Core.SourceGeneratorBase).Assembly.GetName().Version ?? new Version(99, 99, 9999);
            Version thisVersion = typeof(DTOMaker.SrcGen.JsonNewtonSoft.JsonNSSourceGenerator).Assembly.GetName().Version ?? new Version(0, 0, 0);

            thisVersion.Major.ShouldBe(coreVersion.Major);
            thisVersion.Minor.ShouldBeGreaterThanOrEqualTo(coreVersion.Minor);
        }

        [Fact]
        public void RuntimeVersionChecks()
        {
            Version coreVersion = typeof(DTOMaker.Runtime.IEntityBase).Assembly.GetName().Version ?? new Version(99, 99, 9999);
            Version thisVersion = typeof(DTOMaker.Runtime.JsonNewtonSoft.SerializationHelpers).Assembly.GetName().Version ?? new Version(0, 0, 0);

            coreVersion.ToString().ShouldBe("1.2.0.0");
            thisVersion.ToString().ShouldBe("1.2.0.0");

            //thisVersion.Major.ShouldBe(coreVersion.Major);
            //thisVersion.Minor.ShouldBeGreaterThanOrEqualTo(coreVersion.Minor);
        }

    }
}
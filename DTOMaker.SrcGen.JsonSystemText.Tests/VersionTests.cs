using Shouldly;
using System;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.Tests
{
    public class VersionTests
    {
        //[Fact]
        //public void ModelsVersionChecks()
        //{
        //    Version coreVersion = typeof(DTOMaker.Models.EntityAttribute).Assembly.GetName().Version ?? new Version(99, 99, 9999);
        //    Version thisVersion = typeof(DTOMaker.Models.JsonSystemText.xxxxxxAttribute).Assembly.GetName().Version ?? new Version(0, 0, 0);

        //    thisVersion.Major.ShouldBe(coreVersion.Major);
        //    thisVersion.Minor.ShouldBeGreaterThanOrEqualTo(coreVersion.Minor);
        //}

        [Fact]
        public void SrcGenVersionChecks()
        {
            Version coreVersion = typeof(DTOMaker.SrcGen.Core.SourceGeneratorBase).Assembly.GetName().Version ?? new Version(99, 99, 9999);
            Version thisVersion = typeof(DTOMaker.SrcGen.JsonSystemText.JsonSTSourceGenerator).Assembly.GetName().Version ?? new Version(0, 0, 0);

            thisVersion.Major.ShouldBe(coreVersion.Major);
            thisVersion.Minor.ShouldBeGreaterThanOrEqualTo(coreVersion.Minor);
        }

        [Fact]
        public void RuntimeVersionChecks()
        {
            Version coreVersion = typeof(DTOMaker.Runtime.IEntityBase).Assembly.GetName().Version ?? new Version(99, 99, 9999);
            Version thisVersion = typeof(DTOMaker.Runtime.JsonSystemText.PairOfInt16Converter).Assembly.GetName().Version ?? new Version(0, 0, 0);

            coreVersion.ToString().ShouldBe("0.72.0.0");
            thisVersion.ToString().ShouldBe("1.1.0.0");

            //thisVersion.Major.ShouldBe(coreVersion.Major);
            //thisVersion.Minor.ShouldBeGreaterThanOrEqualTo(coreVersion.Minor);
        }

    }
}
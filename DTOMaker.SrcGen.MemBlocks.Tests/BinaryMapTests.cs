using DTOMaker.SrcGen.MemBlocks.BlockLayout;
using Shouldly;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.Tests;

public class BinaryMapTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(20, 1024 * 128 * 8)]
    [InlineData(29, 1024 * 1024 * 512)]
    [InlineData(30, 1024 * 1024 * 1024)]
    public void PowerOf2(sbyte value, int expected)
    {
        int result = BinaryMap.PowerOf2(value);
        result.ShouldBe(expected);
    }

    [Fact]
    public void Create()
    {
        var map = BinaryMap.Empty;
        map.SizeInBits.ShouldBe(8);
        map.State.ShouldBe(Fill.Zero);
    }

    [Fact]
    public void CreateAndAdd1Bit()
    {
        var map = BinaryMap.Empty;
        bool found;
        int offset;
        map.SizeInBits.ShouldBe(8);
        map.State.ShouldBe(Fill.Zero);
        {
            (found, map, offset) = map.AssignField(1);
            found.ShouldBeTrue();
            map.SizeInBits.ShouldBe(8);
            map.State.ShouldBe(Fill.Part);
            offset.ShouldBe(0);
        }
    }

    [Fact]
    public void CreateAndAdd8Bits()
    {
        var map = BinaryMap.Empty;
        bool found;
        int offset;
        map.SizeInBits.ShouldBe(8);
        map.State.ShouldBe(Fill.Zero);
        for (int i = 0; i < 8; i++)
        {
            (found, map, offset) = map.AssignField(1);
            found.ShouldBeTrue();
            map.SizeInBits.ShouldBe(8);
            offset.ShouldBe(i);
        }
        map.State.ShouldBe(Fill.Full);
    }

    [Fact]
    public void CreateAndAutoExpand()
    {
        var map = BinaryMap.Empty;
        bool found;
        int offset;
        map.SizeInBits.ShouldBe(8);
        map.Sizes.ShouldBe((1, 0));
        {
            // add a 1 bit field
            (found, map, offset) = map.AssignField(1);
            found.ShouldBeTrue();
            map.SizeInBits.ShouldBe(8);
            map.Sizes.ShouldBe((1, 0));
            map.State.ShouldBe(Fill.Part);
            offset.ShouldBe(0);
        }
        {
            // add a 4 bit field
            (found, map, offset) = map.AssignField(4);
            found.ShouldBeTrue();
            map.SizeInBits.ShouldBe(8);
            map.State.ShouldBe(Fill.Part);
            offset.ShouldBe(4);
        }
        {
            // add a 2 bit field
            (found, map, offset) = map.AssignField(2);
            found.ShouldBeTrue();
            map.SizeInBits.ShouldBe(8);
            map.State.ShouldBe(Fill.Part);
            offset.ShouldBe(2);
        }
        {
            // add a 1 bit field
            (found, map, offset) = map.AssignField(1);
            found.ShouldBeTrue();
            map.SizeInBits.ShouldBe(8);
            map.State.ShouldBe(Fill.Full);
            offset.ShouldBe(1);
        }
        {
            // add a 1 byte field
            (found, map, offset) = map.AssignField(8);
            found.ShouldBeTrue();
            map.SizeInBits.ShouldBe(16);
            map.State.ShouldBe(Fill.Full);
            offset.ShouldBe(8);
        }
    }
}

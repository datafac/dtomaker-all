using DTOMaker.Runtime.NetStrux;
using Shouldly;
using System;
using Xunit;

namespace Testing.PerfBench.UnitTests
{
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
            int offset;
            map.SizeInBits.ShouldBe(8);
            map.State.ShouldBe(Fill.Zero);
            {
                (map, offset) = map.AssignField(1);
                map.SizeInBits.ShouldBe(8);
                map.State.ShouldBe(Fill.Part);
                offset.ShouldBe(0);
            }
        }

        [Fact]
        public void CreateAndAdd8Bits()
        {
            var map = BinaryMap.Empty;
            int offset;
            map.SizeInBits.ShouldBe(8);
            map.State.ShouldBe(Fill.Zero);
            for (int i = 0; i < 8; i++)
            {
                (map, offset) = map.AssignField(1);
                map.SizeInBits.ShouldBe(8);
                offset.ShouldBe(i);
            }
            map.State.ShouldBe(Fill.Full);
        }

        [Fact]
        public void CreateAndAutoExpand()
        {
            var map = BinaryMap.Empty;
            int offset;
            map.SizeInBits.ShouldBe(8);
            map.Sizes.ShouldBe((1, 0));
            {
                // add a 1 bit field
                (map, offset) = map.AssignField(1);
                map.SizeInBits.ShouldBe(8);
                map.Sizes.ShouldBe((1, 0));
                map.State.ShouldBe(Fill.Part);
                offset.ShouldBe(0);
            }
            {
                // add a 4 bit field
                (map, offset) = map.AssignField(4);
                map.SizeInBits.ShouldBe(8);
                map.State.ShouldBe(Fill.Part);
                offset.ShouldBe(4);
            }
            {
                // add a 2 bit field
                (map, offset) = map.AssignField(2);
                map.SizeInBits.ShouldBe(8);
                map.State.ShouldBe(Fill.Part);
                offset.ShouldBe(2);
            }
            {
                // add a 1 bit field
                (map, offset) = map.AssignField(1);
                map.SizeInBits.ShouldBe(8);
                map.State.ShouldBe(Fill.Full);
                offset.ShouldBe(1);
            }
            {
                // add a 1 byte field
                (map, offset) = map.AssignField(8);
                map.SizeInBits.ShouldBe(16);
                map.State.ShouldBe(Fill.Full);
                offset.ShouldBe(8);
            }
        }
    }

    public class BlockMapBuilderTests
    {
        [Fact]
        public void Create()
        {
            var blockMap = BlockMap.Empty;
            blockMap.BlockSize.ShouldBe(0);
            blockMap.Fields.Length.ShouldBe(0);
            blockMap.IsValid(true).ShouldBeTrue();
        }

        [Fact]
        public void Define_FieldDef_Simple()
        {
            BlockMap blockMap = BlockMap.Empty
                .AddField<Int64>()
                .AddField<bool>()
                .AddField<Int64>()
                ;

            blockMap.IsValid(true).ShouldBeTrue();
            blockMap.Fields.Length.ShouldBe(3);

            var field0 = blockMap.Fields[0];
            field0.ValueOffset.ShouldBe(0);
            field0.ValueLength.ShouldBe(8);

            var field1 = blockMap.Fields[1];
            field1.ValueOffset.ShouldBe(8);
            field1.ValueLength.ShouldBe(1);

            var field2 = blockMap.Fields[2];
            field2.ValueOffset.ShouldBe(16);
            field2.ValueLength.ShouldBe(8);

            blockMap.BlockSize.ShouldBe(32);
        }

        [Fact]
        public void Define_FieldDef()
        {
            BlockMap blockMap = BlockMap.Empty
                .AddField<bool>()
                .AddField<byte>()
                .AddField<sbyte>()
                .AddField<int>()
                .AddField<long>()
                .AddField<string>()
                .AddField<float>()
                ;

            blockMap.IsValid(true).ShouldBeTrue();
            blockMap.Fields.Length.ShouldBe(7);

            var field0 = blockMap.Fields[0];
            field0.ValueOffset.ShouldBe(0);
            field0.ValueLength.ShouldBe(1);

            var field1 = blockMap.Fields[1];
            field1.ValueOffset.ShouldBe(1);
            field1.ValueLength.ShouldBe(1);

            var field2 = blockMap.Fields[2];
            field2.ValueOffset.ShouldBe(2);
            field2.ValueLength.ShouldBe(1);

            var field3 = blockMap.Fields[3];
            field3.ValueOffset.ShouldBe(4);
            field3.ValueLength.ShouldBe(4);

            var field4 = blockMap.Fields[4];
            field4.ValueOffset.ShouldBe(8);
            field4.ValueLength.ShouldBe(8);

            var field5 = blockMap.Fields[5];
            field5.ValueOffset.ShouldBe(64);
            field5.ValueLength.ShouldBe(64);

            var field6 = blockMap.Fields[6];
            field6.ValueOffset.ShouldBe(16);
            field6.ValueLength.ShouldBe(4);

            blockMap.BlockSize.ShouldBe(128);
        }

    }
}

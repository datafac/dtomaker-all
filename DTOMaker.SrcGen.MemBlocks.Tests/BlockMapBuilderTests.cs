using DTOMaker.SrcGen.Core;
using DTOMaker.SrcGen.MemBlocks.BlockLayout;
using Shouldly;
using System;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.Tests;

public class BlockMapBuilderTests
{
    [Fact]
    public void Create()
    {
        var blockMap = new BlockMap();
        blockMap.BlockSize.ShouldBe(0);
        blockMap.Fields.Count.ShouldBe(0);
        blockMap.IsValid(true).ShouldBeTrue();
    }

    [Theory]
    [InlineData(NativeType.SByte, 1)]
    [InlineData(NativeType.Byte, 1)]
    [InlineData(NativeType.Boolean, 1)]
    [InlineData(NativeType.Int16, 2)]
    [InlineData(NativeType.UInt16, 2)]
    [InlineData(NativeType.Char, 2)]
    [InlineData(NativeType.Half, 2)]
    [InlineData(NativeType.Int32, 4)]
    [InlineData(NativeType.UInt32, 4)]
    [InlineData(NativeType.PairOfInt16, 4)]
    [InlineData(NativeType.Single, 4)]
    [InlineData(NativeType.Int64, 8)]
    [InlineData(NativeType.UInt64, 8)]
    [InlineData(NativeType.PairOfInt32, 8)]
    [InlineData(NativeType.Double, 8)]
    [InlineData(NativeType.Int128, 16)]
    [InlineData(NativeType.UInt128, 16)]
    [InlineData(NativeType.PairOfInt64, 16)]
    [InlineData(NativeType.QuadOfInt32, 16)]
    [InlineData(NativeType.Guid, 16)]
    [InlineData(NativeType.Decimal, 16)]
    //[InlineData(NativeType.RawB10, 16)]
    //[InlineData(NativeType.RawB20, 32)]
    //[InlineData(NativeType.RawB40, 64)]
    //[InlineData(NativeType.RawB80, 128)]
    public void Add1stField(NativeType fieldType, int fieldLength)
    {
        var commands = ImmutableList<ExternalFieldDef>.Empty
            .Add(new ExternalFieldDef("Field1", 0, fieldLength, 1, null));

        BlockMap blockMap = new BlockMapBuilder().AddFields(commands).Build();
        blockMap.BlockSize.ShouldBe(fieldLength);
        blockMap.Fields.Count.ShouldBe(1);
        FieldDef field0 = blockMap.Fields.Array[0];
        field0.Offset.ShouldBe(0);
        field0.Length.ShouldBe(fieldLength);
        blockMap.IsValid(true).ShouldBeTrue();
    }

    [Fact]
    public void ProcessEmptyModifyFieldBlockMapRequest()
    {
        var commands = ImmutableList<ExternalFieldDef>.Empty;
        BlockMap blockMap = new BlockMapBuilder().AddFields(commands).Build();
        blockMap.BlockSize.ShouldBe(0);
        blockMap.Fields.Count.ShouldBe(0);
        blockMap.IsValid(true).ShouldBeTrue();
    }

    [Theory]
    [InlineData(NativeType.SByte, 2, 1, 1)]
    [InlineData(NativeType.Byte, 2, 1, 1)]
    [InlineData(NativeType.Boolean, 2, 1, 1)]
    [InlineData(NativeType.Int16, 4, 2, 2)]
    [InlineData(NativeType.UInt16, 4, 2, 2)]
    [InlineData(NativeType.Char, 4, 2, 2)]
    [InlineData(NativeType.Half, 4, 2, 2)]
    [InlineData(NativeType.Int32, 8, 4, 4)]
    [InlineData(NativeType.UInt32, 8, 4, 4)]
    [InlineData(NativeType.PairOfInt16, 8, 4, 4)]
    [InlineData(NativeType.Single, 8, 4, 4)]
    [InlineData(NativeType.Int64, 16, 8, 8)]
    [InlineData(NativeType.UInt64, 16, 8, 8)]
    [InlineData(NativeType.PairOfInt32, 16, 8, 8)]
    [InlineData(NativeType.Double, 16, 8, 8)]
    [InlineData(NativeType.Int128, 32, 16, 16)]
    [InlineData(NativeType.UInt128, 32, 16, 16)]
    [InlineData(NativeType.PairOfInt64, 32, 16, 16)]
    [InlineData(NativeType.QuadOfInt32, 32, 16, 16)]
    [InlineData(NativeType.Guid, 32, 16, 16)]
    [InlineData(NativeType.Decimal, 32, 16, 16)]
    [InlineData(NativeType.String, 128, 64, 64)]
    [InlineData(NativeType.Binary, 128, 64, 64)]
    //[InlineData(NativeType.RawB10, 32, 16, 16)]
    //[InlineData(NativeType.RawB20, 64, 32, 32)]
    //[InlineData(NativeType.RawB40, 128, 64, 64)]
    //[InlineData(NativeType.RawB80, 256, 128, 128)]
    public void Add2ndField(NativeType fieldType, int expectedBlockSize, int expectedOffset, int expectedLength)
    {
        var commands = ImmutableList<ExternalFieldDef>.Empty
            .Add(new ExternalFieldDef("Field1", 0, 1, 1, null))
            .Add(new ExternalFieldDef("Field2", 0, expectedLength, 2, null));

        BlockMap blockMap = new BlockMapBuilder().AddFields(commands).Build();
        blockMap.BlockSize.ShouldBe(expectedBlockSize);
        blockMap.Fields.Count.ShouldBe(2);
        FieldDef field0 = blockMap.Fields.Array[0];
        field0.Offset.ShouldBe(0);
        field0.Length.ShouldBe(1);
        FieldDef field1 = blockMap.Fields.Array[1];
        field1.Offset.ShouldBe(expectedOffset);
        field1.Length.ShouldBe(expectedLength);
        blockMap.IsValid(true).ShouldBeTrue();
    }

    [Fact]
    public void Define_FieldDef()
    {
        var commands = ImmutableList<ExternalFieldDef>.Empty
            .Add(new ExternalFieldDef("Reserved0", 0, 1, 1, null))
            .Add(new ExternalFieldDef("Reserved1", 0, 1, 2, null))
            .Add(new ExternalFieldDef("Logical1", 0, 1, 3, null))
            .Add(new ExternalFieldDef("Numeric1", 0, 4, 4, null))
            .Add(new ExternalFieldDef("Numeric2", 0, 8, 5, null))
            .Add(new ExternalFieldDef("Textual1", 0, 64, 6, null))
            .Add(new ExternalFieldDef("NativeType", 0, 4, 7, null));
        BlockMap blockMap = new BlockMapBuilder().AddFields(commands).Build();

        blockMap.BlockSize.ShouldBe(128);
        blockMap.Fields.Count.ShouldBe(7);
        var field0 = blockMap.Fields.Array[0];
        field0.Offset.ShouldBe(0);
        field0.Length.ShouldBe(1);

        var field1 = blockMap.Fields.Array[1];
        field1.Offset.ShouldBe(1);
        field1.Length.ShouldBe(1);

        var field2 = blockMap.Fields.Array[2];
        field2.Offset.ShouldBe(2);
        field2.Length.ShouldBe(1);

        var field3 = blockMap.Fields.Array[3];
        field3.Offset.ShouldBe(4);
        field3.Length.ShouldBe(4);

        var field4 = blockMap.Fields.Array[4];
        field4.Offset.ShouldBe(8);
        field4.Length.ShouldBe(8);

        var field5 = blockMap.Fields.Array[5];
        field5.Offset.ShouldBe(64);
        field5.Length.ShouldBe(64);

        var field6 = blockMap.Fields.Array[6];
        field6.Offset.ShouldBe(16);
        field6.Length.ShouldBe(4);

        blockMap.IsValid(true).ShouldBeTrue();

        // field paths
        CalcPathToField(blockMap, 0).ShouldBe(".A.A.A.A.A.A.A");
        CalcPathToField(blockMap, 1).ShouldBe(".A.A.A.A.A.A.B");
        CalcPathToField(blockMap, 2).ShouldBe(".A.A.A.A.A.B.A");
        CalcPathToField(blockMap, 3).ShouldBe(".A.A.A.A.B");
        CalcPathToField(blockMap, 4).ShouldBe(".A.A.A.B");
        CalcPathToField(blockMap, 5).ShouldBe(".B");
        CalcPathToField(blockMap, 6).ShouldBe(".A.A.B.A.A");
    }

    private static string CalcPathToField(BlockMap blockMap, int fieldIndex)
    {
        var blockSize = blockMap.BlockSize;
        var field = blockMap.Fields.Array[fieldIndex];
        var fieldLength = field.Length;
        var offset = field.Offset;
        string result = "";
        while (fieldLength < blockSize)
        {
            int halfSize = (int)(blockSize / 2);
            if (offset < halfSize)
            {
                result += ".A";
            }
            else
            {
                result += ".B";
                offset = (int)(offset - halfSize);
            }

            blockSize = halfSize;
        }

        return result;
    }

    [Fact]
    public void AddTooManyFieldsThrows()
    {
        const int MaxFields = (8 * 1024) / 64; // size of Binary field type is 64 bytes
        var commands = ImmutableList<FieldDef>.Empty
            .AddRange(
                Enumerable.Range(0, MaxFields)
                .Select<int, ExternalFieldDef>(i => new ExternalFieldDef($"Field{i:D5}", 0, 64, i + 1, null)));
        BlockMap blockMap = new BlockMapBuilder().AddFields(commands).Build();

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            BlockMap extendedMap = blockMap.ToBuilder()
                .AddField(new ExternalFieldDef("Field99999", 0, 1, 0, null))
                .Build();
        });
        ex.Message.ShouldBe("Cannot expand block map beyond 8KiB (yet)");
    }
}

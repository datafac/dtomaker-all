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

    [Fact]
    public void AddInvalidFieldTypeThrows()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var commands = ImmutableList<FieldCommand>.Empty
                .Add(new AddFieldCommand( 1,"Field1",(FieldType)99,false));

            var response = new BlockMapBuilder(null).AddCommands(commands).Build();
        });
        ex.Message.ShouldStartWith("FieldType(99)");
    }

    [Theory]
    [InlineData(FieldType.SByte, 1)]
    [InlineData(FieldType.Byte, 1)]
    [InlineData(FieldType.Boolean, 1)]
    [InlineData(FieldType.Int16, 2)]
    [InlineData(FieldType.UInt16, 2)]
    [InlineData(FieldType.Char, 2)]
    [InlineData(FieldType.Half, 2)]
    [InlineData(FieldType.Int32, 4)]
    [InlineData(FieldType.UInt32, 4)]
    [InlineData(FieldType.PairOfInt16, 4)]
    [InlineData(FieldType.Single, 4)]
    [InlineData(FieldType.Int64, 8)]
    [InlineData(FieldType.UInt64, 8)]
    [InlineData(FieldType.PairOfInt32, 8)]
    [InlineData(FieldType.Double, 8)]
    [InlineData(FieldType.Int128, 16)]
    [InlineData(FieldType.UInt128, 16)]
    [InlineData(FieldType.PairOfInt64, 16)]
    [InlineData(FieldType.QuadOfInt32, 16)]
    [InlineData(FieldType.Guid, 16)]
    [InlineData(FieldType.Decimal, 16)]
    [InlineData(FieldType.RawB10, 16)]
    [InlineData(FieldType.RawB20, 32)]
    [InlineData(FieldType.RawB40, 64)]
    [InlineData(FieldType.RawB80, 128)]
    public void Add1stField(FieldType fieldType, int expectedLength)
    {
        var commands = ImmutableList<FieldCommand>.Empty
            .Add(new AddFieldCommand(1,"Field1",fieldType, false));

        BlockMap blockMap = new BlockMapBuilder(null).AddCommands(commands).Build();
        blockMap.BlockSize.ShouldBe(expectedLength);
        blockMap.Fields.Count.ShouldBe(1);
        FieldDef field0 = blockMap.Fields.Array[0];
        field0.BlockOffset.ShouldBe(0);
        field0.FieldLength.ShouldBe(expectedLength);
        blockMap.IsValid(true).ShouldBeTrue();
    }

    [Fact]
    public void ProcessEmptyModifyBlockMapRequest()
    {
        var commands = ImmutableList<FieldCommand>.Empty;
        BlockMap blockMap = new BlockMapBuilder(null).AddCommands(commands).Build();
        blockMap.BlockSize.ShouldBe(1);
        blockMap.Fields.Count.ShouldBe(0);
        blockMap.IsValid(true).ShouldBeTrue();
    }

    [Theory]
    [InlineData(FieldType.SByte, 2, 1, 1)]
    [InlineData(FieldType.Byte, 2, 1, 1)]
    [InlineData(FieldType.Boolean, 2, 1, 1)]
    [InlineData(FieldType.Int16, 4, 2, 2)]
    [InlineData(FieldType.UInt16, 4, 2, 2)]
    [InlineData(FieldType.Char, 4, 2, 2)]
    [InlineData(FieldType.Half, 4, 2, 2)]
    [InlineData(FieldType.Int32, 8, 4, 4)]
    [InlineData(FieldType.UInt32, 8, 4, 4)]
    [InlineData(FieldType.PairOfInt16, 8, 4, 4)]
    [InlineData(FieldType.Single, 8, 4, 4)]
    [InlineData(FieldType.Int64, 16, 8, 8)]
    [InlineData(FieldType.UInt64, 16, 8, 8)]
    [InlineData(FieldType.PairOfInt32, 16, 8, 8)]
    [InlineData(FieldType.Double, 16, 8, 8)]
    [InlineData(FieldType.Int128, 32, 16, 16)]
    [InlineData(FieldType.UInt128, 32, 16, 16)]
    [InlineData(FieldType.PairOfInt64, 32, 16, 16)]
    [InlineData(FieldType.QuadOfInt32, 32, 16, 16)]
    [InlineData(FieldType.Guid, 32, 16, 16)]
    [InlineData(FieldType.Decimal, 32, 16, 16)]
    [InlineData(FieldType.RawB10, 32, 16, 16)]
    [InlineData(FieldType.RawB20, 64, 32, 32)]
    [InlineData(FieldType.RawB40, 128, 64, 64)]
    [InlineData(FieldType.RawB80, 256, 128, 128)]
    public void Add2ndField(FieldType fieldType, int expectedBlockSize, int expectedOffset, int expectedLength)
    {
        var commands = ImmutableList<FieldCommand>.Empty
            .Add(new AddFieldCommand(1,"Field1",FieldType.Boolean,false))
            .Add(new AddFieldCommand(2,"Field2",fieldType,false));

        BlockMap blockMap = new BlockMapBuilder(null).AddCommands(commands).Build();
        blockMap.BlockSize.ShouldBe(expectedBlockSize);
        blockMap.Fields.Count.ShouldBe(2);
        FieldDef field0 = blockMap.Fields.Array[0];
        field0.BlockOffset.ShouldBe(0);
        field0.FieldLength.ShouldBe(1);
        FieldDef field1 = blockMap.Fields.Array[1];
        field1.BlockOffset.ShouldBe(expectedOffset);
        field1.FieldLength.ShouldBe(expectedLength);
        blockMap.IsValid(true).ShouldBeTrue();
    }

    [Fact]
    public void Define_FieldDef()
    {
        var commands = ImmutableList<FieldCommand>.Empty
            .Add(new AddFieldCommand(1, "Reserved0", FieldType.Byte, false))
            .Add(new AddFieldCommand(2, "Reserved1", FieldType.Byte, false))
            .Add(new AddFieldCommand(3, "Logical1", FieldType.Boolean, false))
            .Add(new AddFieldCommand(4, "Numeric1", FieldType.Int32, false))
            .Add(new AddFieldCommand(5, "Numeric2", FieldType.Int64, false))
            .Add(new AddFieldCommand(6, "Textual1", FieldType.String, false))
            .Add(new AddFieldCommand(7, "FieldType", FieldType.Int32, false));
        BlockMap blockMap = new BlockMapBuilder(null).AddCommands(commands).Build();

        blockMap.BlockSize.ShouldBe(128);
        blockMap.Fields.Count.ShouldBe(7);
        var field0 = blockMap.Fields.Array[0];
        field0.BlockOffset.ShouldBe(0);
        field0.FieldLength.ShouldBe(1);
        field0.FieldName.ShouldBe("Reserved0");
        field0.FieldType.ShouldBe(FieldType.Byte);

        var field1 = blockMap.Fields.Array[1];
        field1.BlockOffset.ShouldBe(1);
        field1.FieldLength.ShouldBe(1);
        field1.FieldName.ShouldBe("Reserved1");
        field1.FieldType.ShouldBe(FieldType.Byte);

        var field2 = blockMap.Fields.Array[2];
        field2.BlockOffset.ShouldBe(2);
        field2.FieldLength.ShouldBe(1);
        field2.FieldName.ShouldBe("Logical1");
        field2.FieldType.ShouldBe(FieldType.Boolean);

        var field3 = blockMap.Fields.Array[3];
        field3.BlockOffset.ShouldBe(4);
        field3.FieldLength.ShouldBe(4);
        field3.FieldName.ShouldBe("Numeric1");
        field3.FieldType.ShouldBe(FieldType.Int32);

        var field4 = blockMap.Fields.Array[4];
        field4.BlockOffset.ShouldBe(8);
        field4.FieldLength.ShouldBe(8);
        field4.FieldName.ShouldBe("Numeric2");
        field4.FieldType.ShouldBe(FieldType.Int64);

        var field5 = blockMap.Fields.Array[5];
        field5.BlockOffset.ShouldBe(64);
        field5.FieldLength.ShouldBe(64);
        field5.FieldName.ShouldBe("Textual1");
        field5.FieldType.ShouldBe(FieldType.String);

        var field6 = blockMap.Fields.Array[6];
        field6.BlockOffset.ShouldBe(16);
        field6.FieldLength.ShouldBe(4);
        field6.FieldName.ShouldBe("FieldType");
        field6.FieldType.ShouldBe(FieldType.Int32);

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
        var fieldLength = field.FieldLength;
        var offset = field.BlockOffset;
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
        const int MaxFields = (8 * 1024) / 128; // size of RawB80 field type is 128 bytes
        var commands = ImmutableList<FieldCommand>.Empty
            .AddRange(
                Enumerable.Range(0, MaxFields)
                .Select<int, FieldCommand>(i => new AddFieldCommand(1, $"Field{i:D3}", FieldType.RawB80, false)));
        BlockMap blockMap = new BlockMapBuilder(null).AddCommands(commands).Build();

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var moreCommands = ImmutableList<FieldCommand>.Empty
                .Add(new AddFieldCommand(1, "Field99999", FieldType.Boolean, false));
            BlockMap extendedMap = new BlockMapBuilder(blockMap).AddCommands(moreCommands).Build();
        });
        ex.Message.ShouldBe("Cannot expand block map beyond 8KiB (yet)");
    }
}

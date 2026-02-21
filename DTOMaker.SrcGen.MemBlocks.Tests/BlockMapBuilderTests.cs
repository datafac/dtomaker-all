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

    [Fact]
    public void AddInvalidFieldTypeThrows()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var commands = ImmutableList<FieldCommand>.Empty
                .Add(new AddFieldCommand( 1,"Field1",(NativeType)99,false));

            var response = new BlockMapBuilder(null).AddCommands(commands).Build();
        });
        ex.Message.ShouldStartWith("NativeType(99)");
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
    public void Add1stField(NativeType fieldType, int expectedLength)
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
    //[InlineData(NativeType.RawB10, 32, 16, 16)]
    //[InlineData(NativeType.RawB20, 64, 32, 32)]
    //[InlineData(NativeType.RawB40, 128, 64, 64)]
    //[InlineData(NativeType.RawB80, 256, 128, 128)]
    public void Add2ndField(NativeType fieldType, int expectedBlockSize, int expectedOffset, int expectedLength)
    {
        var commands = ImmutableList<FieldCommand>.Empty
            .Add(new AddFieldCommand(1,"Field1",NativeType.Boolean,false))
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
            .Add(new AddFieldCommand(1, "Reserved0", NativeType.Byte, false))
            .Add(new AddFieldCommand(2, "Reserved1", NativeType.Byte, false))
            .Add(new AddFieldCommand(3, "Logical1", NativeType.Boolean, false))
            .Add(new AddFieldCommand(4, "Numeric1", NativeType.Int32, false))
            .Add(new AddFieldCommand(5, "Numeric2", NativeType.Int64, false))
            .Add(new AddFieldCommand(6, "Textual1", NativeType.String, false))
            .Add(new AddFieldCommand(7, "NativeType", NativeType.Int32, false));
        BlockMap blockMap = new BlockMapBuilder(null).AddCommands(commands).Build();

        blockMap.BlockSize.ShouldBe(128);
        blockMap.Fields.Count.ShouldBe(7);
        var field0 = blockMap.Fields.Array[0];
        field0.BlockOffset.ShouldBe(0);
        field0.FieldLength.ShouldBe(1);
        field0.FieldName.ShouldBe("Reserved0");
        field0.NativeType.ShouldBe(NativeType.Byte);

        var field1 = blockMap.Fields.Array[1];
        field1.BlockOffset.ShouldBe(1);
        field1.FieldLength.ShouldBe(1);
        field1.FieldName.ShouldBe("Reserved1");
        field1.NativeType.ShouldBe(NativeType.Byte);

        var field2 = blockMap.Fields.Array[2];
        field2.BlockOffset.ShouldBe(2);
        field2.FieldLength.ShouldBe(1);
        field2.FieldName.ShouldBe("Logical1");
        field2.NativeType.ShouldBe(NativeType.Boolean);

        var field3 = blockMap.Fields.Array[3];
        field3.BlockOffset.ShouldBe(4);
        field3.FieldLength.ShouldBe(4);
        field3.FieldName.ShouldBe("Numeric1");
        field3.NativeType.ShouldBe(NativeType.Int32);

        var field4 = blockMap.Fields.Array[4];
        field4.BlockOffset.ShouldBe(8);
        field4.FieldLength.ShouldBe(8);
        field4.FieldName.ShouldBe("Numeric2");
        field4.NativeType.ShouldBe(NativeType.Int64);

        var field5 = blockMap.Fields.Array[5];
        field5.BlockOffset.ShouldBe(64);
        field5.FieldLength.ShouldBe(64);
        field5.FieldName.ShouldBe("Textual1");
        field5.NativeType.ShouldBe(NativeType.String);

        var field6 = blockMap.Fields.Array[6];
        field6.BlockOffset.ShouldBe(16);
        field6.FieldLength.ShouldBe(4);
        field6.FieldName.ShouldBe("NativeType");
        field6.NativeType.ShouldBe(NativeType.Int32);

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
        const int MaxFields = (8 * 1024) / 64; // size of Binary field type is 64 bytes
        var commands = ImmutableList<FieldCommand>.Empty
            .AddRange(
                Enumerable.Range(0, MaxFields)
                .Select<int, FieldCommand>(i => new AddFieldCommand(1, $"Field{i:D5}", NativeType.Binary, false)));
        BlockMap blockMap = new BlockMapBuilder(null).AddCommands(commands).Build();

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var moreCommands = ImmutableList<FieldCommand>.Empty
                .Add(new AddFieldCommand(1, "Field99999", NativeType.Boolean, false));
            BlockMap extendedMap = new BlockMapBuilder(blockMap).AddCommands(moreCommands).Build();
        });
        ex.Message.ShouldBe("Cannot expand block map beyond 8KiB (yet)");
    }
}

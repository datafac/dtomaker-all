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
    [InlineData(FieldType.SInt01, 1)]
    [InlineData(FieldType.UInt01, 1)]
    [InlineData(FieldType.Bool01, 1)]
    [InlineData(FieldType.SInt02, 2)]
    [InlineData(FieldType.UInt02, 2)]
    [InlineData(FieldType.Char02, 2)]
    [InlineData(FieldType.Real02, 2)]
    [InlineData(FieldType.SInt04, 4)]
    [InlineData(FieldType.UInt04, 4)]
    [InlineData(FieldType.Real04, 4)]
    [InlineData(FieldType.SInt08, 8)]
    [InlineData(FieldType.UInt08, 8)]
    [InlineData(FieldType.Real08, 8)]
    [InlineData(FieldType.SInt10, 16)]
    [InlineData(FieldType.UInt10, 16)]
    [InlineData(FieldType.Guid10, 16)]
    [InlineData(FieldType.Deci10, 16)]
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
    [InlineData(FieldType.SInt01, 2, 1, 1)]
    [InlineData(FieldType.UInt01, 2, 1, 1)]
    [InlineData(FieldType.Bool01, 2, 1, 1)]
    [InlineData(FieldType.SInt02, 4, 2, 2)]
    [InlineData(FieldType.UInt02, 4, 2, 2)]
    [InlineData(FieldType.Char02, 4, 2, 2)]
    [InlineData(FieldType.Real02, 4, 2, 2)]
    [InlineData(FieldType.SInt04, 8, 4, 4)]
    [InlineData(FieldType.UInt04, 8, 4, 4)]
    [InlineData(FieldType.Real04, 8, 4, 4)]
    [InlineData(FieldType.SInt08, 16, 8, 8)]
    [InlineData(FieldType.UInt08, 16, 8, 8)]
    [InlineData(FieldType.Real08, 16, 8, 8)]
    [InlineData(FieldType.SInt10, 32, 16, 16)]
    [InlineData(FieldType.UInt10, 32, 16, 16)]
    [InlineData(FieldType.Guid10, 32, 16, 16)]
    [InlineData(FieldType.Deci10, 32, 16, 16)]
    [InlineData(FieldType.RawB10, 32, 16, 16)]
    [InlineData(FieldType.RawB20, 64, 32, 32)]
    [InlineData(FieldType.RawB40, 128, 64, 64)]
    [InlineData(FieldType.RawB80, 256, 128, 128)]
    public void Add2ndField(FieldType fieldType, int expectedBlockSize, int expectedOffset, int expectedLength)
    {
        var commands = ImmutableList<FieldCommand>.Empty
            .Add(new AddFieldCommand(1,"Field1",FieldType.Bool01,false))
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
            .Add(new AddFieldCommand(1, "Reserved0", FieldType.UInt01, false))
            .Add(new AddFieldCommand(2, "Reserved1", FieldType.UInt01, false))
            .Add(new AddFieldCommand(3, "Logical1", FieldType.Bool01, false))
            .Add(new AddFieldCommand(4, "Numeric1", FieldType.SInt04, false))
            .Add(new AddFieldCommand(5, "Numeric2", FieldType.SInt08, false))
            .Add(new AddFieldCommand(6, "Textual1", FieldType.String, false))
            .Add(new AddFieldCommand(7, "FieldType", FieldType.SInt04, false));
        BlockMap blockMap = new BlockMapBuilder(null).AddCommands(commands).Build();

        blockMap.BlockSize.ShouldBe(128);
        blockMap.Fields.Count.ShouldBe(7);
        var field0 = blockMap.Fields.Array[0];
        field0.BlockOffset.ShouldBe(0);
        field0.FieldLength.ShouldBe(1);
        field0.FieldName.ShouldBe("Reserved0");
        field0.FieldType.ShouldBe(FieldType.UInt01);

        var field1 = blockMap.Fields.Array[1];
        field1.BlockOffset.ShouldBe(1);
        field1.FieldLength.ShouldBe(1);
        field1.FieldName.ShouldBe("Reserved1");
        field1.FieldType.ShouldBe(FieldType.UInt01);

        var field2 = blockMap.Fields.Array[2];
        field2.BlockOffset.ShouldBe(2);
        field2.FieldLength.ShouldBe(1);
        field2.FieldName.ShouldBe("Logical1");
        field2.FieldType.ShouldBe(FieldType.Bool01);

        var field3 = blockMap.Fields.Array[3];
        field3.BlockOffset.ShouldBe(4);
        field3.FieldLength.ShouldBe(4);
        field3.FieldName.ShouldBe("Numeric1");
        field3.FieldType.ShouldBe(FieldType.SInt04);

        var field4 = blockMap.Fields.Array[4];
        field4.BlockOffset.ShouldBe(8);
        field4.FieldLength.ShouldBe(8);
        field4.FieldName.ShouldBe("Numeric2");
        field4.FieldType.ShouldBe(FieldType.SInt08);

        var field5 = blockMap.Fields.Array[5];
        field5.BlockOffset.ShouldBe(64);
        field5.FieldLength.ShouldBe(64);
        field5.FieldName.ShouldBe("Textual1");
        field5.FieldType.ShouldBe(FieldType.String);

        var field6 = blockMap.Fields.Array[6];
        field6.BlockOffset.ShouldBe(16);
        field6.FieldLength.ShouldBe(4);
        field6.FieldName.ShouldBe("FieldType");
        field6.FieldType.ShouldBe(FieldType.SInt04);

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
                .Add(new AddFieldCommand(1, "Field99999", FieldType.Bool01, false));
            BlockMap extendedMap = new BlockMapBuilder(blockMap).AddCommands(moreCommands).Build();
        });
        ex.Message.ShouldBe("Cannot expand block map beyond 8KiB (yet)");
    }
}

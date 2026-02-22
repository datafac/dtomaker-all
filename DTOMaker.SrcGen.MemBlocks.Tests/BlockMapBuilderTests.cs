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
        var commands = ImmutableList<BlockMapRequest>.Empty
            .Add(new BlockMapRequest(1, "Field1", MemberKind.Struct, (NativeType)99));

        var response = new BlockMapBuilder(null).AddRequests(commands).Build();

        response.BlockSize.ShouldBe(0);
        response.Fields.Count.ShouldBe(0);
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
        var commands = ImmutableList<BlockMapRequest>.Empty
            .Add(new BlockMapRequest(1, "Field1", MemberKind.Struct, fieldType));

        BlockMap blockMap = new BlockMapBuilder(null).AddRequests(commands).Build();
        blockMap.BlockSize.ShouldBe(expectedLength);
        blockMap.Fields.Count.ShouldBe(1);
        FieldDef field0 = blockMap.Fields.Array[0];
        field0.Offset.ShouldBe(0);
        field0.Length.ShouldBe(expectedLength);
        blockMap.IsValid(true).ShouldBeTrue();
    }

    [Fact]
    public void ProcessEmptyModifyBlockMapRequest()
    {
        var commands = ImmutableList<BlockMapRequest>.Empty;
        BlockMap blockMap = new BlockMapBuilder(null).AddRequests(commands).Build();
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
    // todo entity, string and binary
    //[InlineData(NativeType.RawB10, 32, 16, 16)]
    //[InlineData(NativeType.RawB20, 64, 32, 32)]
    //[InlineData(NativeType.RawB40, 128, 64, 64)]
    //[InlineData(NativeType.RawB80, 256, 128, 128)]
    public void Add2ndField(NativeType fieldType, int expectedBlockSize, int expectedOffset, int expectedLength)
    {
        var commands = ImmutableList<BlockMapRequest>.Empty
            .Add(new BlockMapRequest(1, "Field1", MemberKind.Struct, NativeType.Boolean))
            .Add(new BlockMapRequest(2, "Field2", MemberKind.Struct, fieldType));

        BlockMap blockMap = new BlockMapBuilder(null).AddRequests(commands).Build();
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
        var commands = ImmutableList<BlockMapRequest>.Empty
            .Add(new BlockMapRequest(1, "Reserved0", MemberKind.Struct, NativeType.Byte))
            .Add(new BlockMapRequest(2, "Reserved1", MemberKind.Struct, NativeType.Byte))
            .Add(new BlockMapRequest(3, "Logical1", MemberKind.Struct, NativeType.Boolean))
            .Add(new BlockMapRequest(4, "Numeric1", MemberKind.Struct, NativeType.Int32))
            .Add(new BlockMapRequest(5, "Numeric2", MemberKind.Struct, NativeType.Int64))
            .Add(new BlockMapRequest(6, "Textual1", MemberKind.Struct, NativeType.String))
            .Add(new BlockMapRequest(7, "NativeType", MemberKind.Struct, NativeType.Int32));
        BlockMap blockMap = new BlockMapBuilder(null).AddRequests(commands).Build();

        blockMap.BlockSize.ShouldBe(128);
        blockMap.Fields.Count.ShouldBe(7);
        var field0 = blockMap.Fields.Array[0];
        field0.Offset.ShouldBe(0);
        field0.Length.ShouldBe(1);
        field0.Name.ShouldBe("Reserved0");

        var field1 = blockMap.Fields.Array[1];
        field1.Offset.ShouldBe(1);
        field1.Length.ShouldBe(1);
        field1.Name.ShouldBe("Reserved1");

        var field2 = blockMap.Fields.Array[2];
        field2.Offset.ShouldBe(2);
        field2.Length.ShouldBe(1);
        field2.Name.ShouldBe("Logical1");

        var field3 = blockMap.Fields.Array[3];
        field3.Offset.ShouldBe(4);
        field3.Length.ShouldBe(4);
        field3.Name.ShouldBe("Numeric1");

        var field4 = blockMap.Fields.Array[4];
        field4.Offset.ShouldBe(8);
        field4.Length.ShouldBe(8);
        field4.Name.ShouldBe("Numeric2");

        var field5 = blockMap.Fields.Array[5];
        field5.Offset.ShouldBe(64);
        field5.Length.ShouldBe(64);
        field5.Name.ShouldBe("Textual1");

        var field6 = blockMap.Fields.Array[6];
        field6.Offset.ShouldBe(16);
        field6.Length.ShouldBe(4);
        field6.Name.ShouldBe("NativeType");

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
        var commands = ImmutableList<BlockMapRequest>.Empty
            .AddRange(
                Enumerable.Range(0, MaxFields)
                .Select<int, BlockMapRequest>(i => new BlockMapRequest(1, $"Field{i:D5}", MemberKind.Struct, NativeType.Binary)));
        BlockMap blockMap = new BlockMapBuilder(null).AddRequests(commands).Build();

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var moreCommands = ImmutableList<BlockMapRequest>.Empty
                .Add(new BlockMapRequest(1, "Field99999", MemberKind.Struct, NativeType.Boolean));
            BlockMap extendedMap = new BlockMapBuilder(blockMap).AddRequests(moreCommands).Build();
        });
        ex.Message.ShouldBe("Cannot expand block map beyond 8KiB (yet)");
    }
}

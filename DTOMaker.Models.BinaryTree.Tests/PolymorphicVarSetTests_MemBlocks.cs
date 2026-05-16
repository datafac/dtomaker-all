using DataFac.Storage.Testing;
using DTOMaker.Runtime;
using DTOMaker.Runtime.MemBlocks;
using Shouldly;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestOrg.TestApp.Models;
using TestOrg.TestApp.Models.MemBlocks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.Models.BinaryTree.Tests;

public static class VerifyHelpers
{
    public static string ToDisplay(this ReadOnlyMemory<byte> buffer)
    {
        var result = new StringBuilder();
        int i = 0;
        foreach (byte b in buffer.Span)
        {
            if (i % 32 == 0)
            {
                if (i > 0) result.AppendLine();
            }
            else
            {
                result.Append('-');
            }
            result.Append(b.ToString("X2"));
            i++;
        }
        result.AppendLine();
        return result.ToString();
    }
}

public class PolymorphicVarSetTests_MemBlocks
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task RoundtripVarString(int valueId)
    {
        string value = valueId switch
        {
            0 => string.Empty,
            1 => "abcdef",
            2 => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
            3 => "0000000000111111111122222222223333333333444444444455555555556666666666777777777788888888889999999999",
            4 => "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            _ => throw new ArgumentOutOfRangeException(nameof(valueId), valueId, null)
        };

        using var dataStore = new TestDataStore();
        VarBase orig = new VarString() { Value = value };
        await orig.Pack(dataStore);

        var buffer = orig.GetBuffer();

        var metadata = new EntityMetadata(buffer);
        metadata.SignatureBits.ShouldBe(0x01025f7c);
        metadata.StructureBits.ShouldBe(0x702);
        metadata.EntityId.ShouldBe(6);

        var copy = VarBase.DeserializeFrom(buffer);
        copy.ShouldNotBeNull();
        await copy.UnpackAll(dataStore);

        copy.ShouldBe(orig);

        string json = buffer.ToDisplay();
        await Verifier.Verify(json);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
#if NET8_0_OR_GREATER
    public async Task RoundtripVarSetNode_Net80(int valueId)
#else
    public async Task RoundtripVarSetNode_Net48(int valueId)
#endif
    {
        string value = valueId switch
        {
            0 => string.Empty,
            1 => "abcdef",
            2 => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
            3 => "0000000000111111111122222222223333333333444444444455555555556666666666777777777788888888889999999999",
            4 => "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            _ => throw new ArgumentOutOfRangeException(nameof(valueId), valueId, null)
        };

        using var dataStore = new TestDataStore();
        VarBase node = new VarString() { Value = value };
        VarSetNode orig = new VarSetNode() { Count = 1, Depth = 0, Key = "abc", Value = node };
        await orig.Pack(dataStore);

        var buffer = orig.GetBuffer();

        var metadata = new EntityMetadata(buffer);
        metadata.SignatureBits.ShouldBe(0x01025f7c);
        metadata.StructureBits.ShouldBe(0x0A1);
        metadata.EntityId.ShouldBe(3);

        var copy = VarSetNode.DeserializeFrom(buffer);
        copy.ShouldNotBeNull();
        await copy.UnpackAll(dataStore);

        copy.ShouldBe(orig);

        string json = buffer.ToDisplay();
        await Verifier.Verify(json);
    }

    [Fact]
    public async Task RoundtripVarSet01_Empty()
    {
        using var dataStore = new TestDataStore();
        var tree = new VarSetNode();
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("a", new VarString() { Value = "abcdef" });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("b", new VarBoolean() { Value = true });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("c", new VarInt64() { Value = 123456L });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("d", new VarInt64() { Value = 234567L });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("e", new VarString() { Value = "ghijkl" });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("f", new VarBoolean() { Value = false });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("g", new VarInt64() { Value = 345678L });
        var orig = new VarSet()
        {
            Root = tree
        };
        await orig.Pack(dataStore);
        var buffer = orig.GetBuffer();

        string json = buffer.ToDisplay();
        await Verifier.Verify(json);

        var copy = new VarSet(buffer);
        copy.ShouldNotBeNull();
        await copy.UnpackAll(dataStore);
        copy.ShouldBe(orig);
    }

    [Fact]
#if NET8_0_OR_GREATER
    public async Task RoundtripVarSet02_1Node_Net80()
#else
    public async Task RoundtripVarSet02_1Node_Net48()
#endif
    {
        using var dataStore = new TestDataStore();
        var tree = new VarSetNode();
        tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("a", new VarString() { Value = "abcdef" });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("b", new VarBoolean() { Value = true });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("c", new VarInt64() { Value = 123456L });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("d", new VarInt64() { Value = 234567L });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("e", new VarString() { Value = "ghijkl" });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("f", new VarBoolean() { Value = false });
        //tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("g", new VarInt64() { Value = 345678L });
        var orig = new VarSet()
        {
            Root = tree
        };
        await orig.Pack(dataStore);
        var buffer = orig.GetBuffer();

        string json = buffer.ToDisplay();
        await Verifier.Verify(json);

        var copy = new VarSet(buffer);
        copy.ShouldNotBeNull();
        await copy.UnpackAll(dataStore);
        copy.ShouldBe(orig);
    }

    [Fact]
#if NET8_0_OR_GREATER
    public async Task RoundtripVarSet_Net80()
#else
    public async Task RoundtripVarSet_Net48()
#endif
    {
        using var dataStore = new TestDataStore();
        var tree = new VarSetNode();
        tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("a", new VarString() { Value = "abcdef" });
        tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("b", new VarBoolean() { Value = true });
        tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("c", new VarInt64() { Value = 123456L });
        tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("d", new VarInt64() { Value = 234567L });
        tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("e", new VarString() { Value = "ghijkl" });
        tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("f", new VarBoolean() { Value = false });
        tree = tree.AddOrUpdate<string, IVarBase, VarSetNode>("g", new VarInt64() { Value = 345678L });
        var orig = new VarSet()
        {
            Root = tree
        };
        await orig.Pack(dataStore);
        var buffer = orig.GetBuffer();

        string json = buffer.ToDisplay();
        await Verifier.Verify(json);

        var copy = new VarSet(buffer);
        copy.ShouldNotBeNull();
        await copy.UnpackAll(dataStore);
        copy.ShouldBe(orig);
    }
}
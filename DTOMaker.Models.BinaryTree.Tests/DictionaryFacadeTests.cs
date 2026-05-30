using Shouldly;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using TestOrg.TestApp.Models;
using Xunit;

namespace DTOMaker.Models.BinaryTree.Tests;

public class DictionaryFacadeTests
{
    private static IDictionary<string, IVarBase> CreateDictionary(ImplKind kind)
    {
        switch (kind)
        {
            case ImplKind.Reference:
                {
                    return new Dictionary<string, IVarBase>();
                }
            case ImplKind.MemBlox2:
                {
                    var node = new TestOrg.TestApp.Models.MemBlocks.VarSet();
                    return new DictionaryFacade<string, IVarBase, TestOrg.TestApp.Models.MemBlocks.VarSetNode>(() => node.Root, n => node.Root = n);
                }
            case ImplKind.MsgPack2:
                {
                    var node = new TestOrg.TestApp.Models.MsgPack2.VarSet();
                    return new DictionaryFacade<string, IVarBase, TestOrg.TestApp.Models.MsgPack2.VarSetNode>(() => node.Root, n => node.Root = n);
                }
            case ImplKind.JsonSystemText:
                {
                    var node = new TestOrg.TestApp.Models.JsonSystemText.VarSet();
                    return new DictionaryFacade<string, IVarBase, TestOrg.TestApp.Models.JsonSystemText.VarSetNode>(() => node.Root, n => node.Root = n);
                }
            case ImplKind.JsonNewtonSoft:
                {
                    var node = new TestOrg.TestApp.Models.JsonNewtonSoft.VarSet();
                    return new DictionaryFacade<string, IVarBase, TestOrg.TestApp.Models.JsonNewtonSoft.VarSetNode>(() => node.Root, n => node.Root = n);
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
    }

    private static IVarString CreateString(ImplKind kind)
    {
        switch (kind)
        {
            case ImplKind.Reference:
                return new TestOrg.TestApp.Models.MemBlocks.VarString() { Value = "someString" };
            case ImplKind.MemBlox2:
                return new TestOrg.TestApp.Models.MemBlocks.VarString() { Value = "someString" };
            case ImplKind.MsgPack2:
                return new TestOrg.TestApp.Models.MsgPack2.VarString() { Value = "someString" };
            case ImplKind.JsonSystemText:
                return new TestOrg.TestApp.Models.JsonSystemText.VarString() { Value = "someString" };
            case ImplKind.JsonNewtonSoft:
                return new TestOrg.TestApp.Models.JsonNewtonSoft.VarString() { Value = "someString" };
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
    }

    [Theory]
    [InlineData(ImplKind.Reference)]
    [InlineData(ImplKind.JsonSystemText)]
    [InlineData(ImplKind.JsonNewtonSoft)]
    [InlineData(ImplKind.MsgPack2)]
    [InlineData(ImplKind.MemBlox2)]
    public void Writable00_Create(ImplKind kind)
    {
        // act
        IDictionary<string, IVarBase> impl = CreateDictionary(kind);

        // assert
        impl.Count.ShouldBe(0);
        impl.ContainsKey("a").ShouldBeFalse();
        impl.TryGetValue("a", out var _).ShouldBeFalse();
        impl.ToArray().ShouldBeEmpty();
    }

    [Theory]
    [InlineData(ImplKind.Reference)]
    [InlineData(ImplKind.JsonSystemText)]
    [InlineData(ImplKind.JsonNewtonSoft)]
    [InlineData(ImplKind.MsgPack2)]
    [InlineData(ImplKind.MemBlox2)]
    public void Writable01_Add(ImplKind kind)
    {
        // arrange
        IDictionary<string, IVarBase> impl = CreateDictionary(kind);
        IVarBase value = CreateString(kind);

        // act
        impl.Add("a", value);

        // assert
        impl.Count.ShouldBe(1);
        impl.ContainsKey("a").ShouldBeTrue();
        impl.TryGetValue("a", out var value2).ShouldBeTrue();
        value2.ShouldNotBeNull();
        value2.Equals(value).ShouldBeTrue();
        impl.ToArray().ShouldHaveSingleItem();
    }

    [Theory]
    [InlineData(ImplKind.Reference)]
    [InlineData(ImplKind.JsonSystemText)]
    [InlineData(ImplKind.JsonNewtonSoft)]
    [InlineData(ImplKind.MsgPack2)]
    [InlineData(ImplKind.MemBlox2)]
    public void Writable02_AddAgain(ImplKind kind)
    {
        // arrange
        IDictionary<string, IVarBase> impl = CreateDictionary(kind);
        IVarBase value = CreateString(kind);
        impl.Add("a", value);

        // act
        var ex = Assert.ThrowsAny<ArgumentException>(() => impl.Add("a", value));
        ex.Message.ShouldStartWith("An item with the same key has already been added.");

        // assert
        impl.Count.ShouldBe(1);
        impl.ToArray().ShouldHaveSingleItem();
        impl.ContainsKey("a").ShouldBeTrue();
        impl.TryGetValue("a", out var value2).ShouldBeTrue();
        value2.ShouldNotBeNull();
        value2.Equals(value).ShouldBeTrue();
    }

    [Theory]
    [InlineData(ImplKind.Reference)]
    [InlineData(ImplKind.JsonSystemText)]
    [InlineData(ImplKind.JsonNewtonSoft)]
    [InlineData(ImplKind.MsgPack2)]
    [InlineData(ImplKind.MemBlox2)]
    public void Writable03_Remove(ImplKind kind)
    {
        // arrange
        IDictionary<string, IVarBase> impl = CreateDictionary(kind);
        IVarBase value = CreateString(kind);
        impl.Add("a", value);

        // act
        bool removed = impl.Remove("a");

        // assert
        removed.ShouldBeTrue();
        impl.ToArray().ShouldBeEmpty();
        impl.Count.ShouldBe(0);
        impl.ContainsKey("a").ShouldBeFalse();
        impl.TryGetValue("a", out var value2).ShouldBeFalse();
        value2.ShouldBeNull();
    }

    [Theory]
    [InlineData(ImplKind.Reference)]
    [InlineData(ImplKind.JsonSystemText)]
    [InlineData(ImplKind.JsonNewtonSoft)]
    [InlineData(ImplKind.MsgPack2)]
    [InlineData(ImplKind.MemBlox2)]
    public void Writable04_RemoveAgain(ImplKind kind)
    {
        // arrange
        IDictionary<string, IVarBase> impl = CreateDictionary(kind);
        IVarBase value = CreateString(kind);
        impl.Add("a", value);
        impl.Remove("a");

        // act
        bool removed = impl.Remove("a");

        // assert
        removed.ShouldBeFalse();
        impl.ToArray().ShouldBeEmpty();
        impl.Count.ShouldBe(0);
        impl.ContainsKey("a").ShouldBeFalse();
        impl.TryGetValue("a", out var value2).ShouldBeFalse();
        value2.ShouldBeNull();
    }

    [Theory]
    [InlineData(ImplKind.Reference)]
    [InlineData(ImplKind.JsonSystemText)]
    [InlineData(ImplKind.JsonNewtonSoft)]
    [InlineData(ImplKind.MsgPack2)]
    [InlineData(ImplKind.MemBlox2)]
    public void Writable05_AddMany(ImplKind kind)
    {
        // arrange
        IDictionary<string, IVarBase> impl = CreateDictionary(kind);
        IVarBase value = CreateString(kind);
        impl.Add("a", value);
        impl.Add("b", value);
        impl.Add("c", value);

        // act
        // assert
        impl.Count.ShouldBe(3);
        impl.Count().ShouldBe(3);
        impl.ToArray().Length.ShouldBe(3);

        impl.Keys.Count().ShouldBe(3);
        impl.Values.Count().ShouldBe(3);
    }

    [Theory]
    [InlineData(ImplKind.Reference)]
    [InlineData(ImplKind.JsonSystemText)]
    [InlineData(ImplKind.JsonNewtonSoft)]
    [InlineData(ImplKind.MsgPack2)]
    [InlineData(ImplKind.MemBlox2)]
    public void Writable06_Clear(ImplKind kind)
    {
        // arrange
        IDictionary<string, IVarBase> impl = CreateDictionary(kind);
        IVarBase value = CreateString(kind);
        impl.Add("a", value);
        impl.Add("b", value);
        impl.Add("c", value);

        // act
        impl.Clear();

        // assert
        impl.Count.ShouldBe(0);
        impl.ContainsKey("a").ShouldBeFalse();
        impl.TryGetValue("a", out var _).ShouldBeFalse();
        impl.ToArray().ShouldBeEmpty();
    }

}

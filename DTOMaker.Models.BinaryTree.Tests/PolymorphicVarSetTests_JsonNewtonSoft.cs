using DataFac.Storage.Testing;
using DTOMaker.Runtime.JsonNewtonSoft;
using Shouldly;
using System.Threading.Tasks;
using TestOrg.TestApp.Models;
using TestOrg.TestApp.Models.JsonNewtonSoft;
using VerifyXunit;
using Xunit;

namespace DTOMaker.Models.BinaryTree.Tests;

public class PolymorphicVarSetTests_JsonNewtonSoft
{
    [Fact]
    public async Task RoundtripVarSet()
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
        orig.Freeze();

        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<VarSet>();
        copy.ShouldNotBeNull();
        copy.IsFrozen.ShouldBeFalse();
        copy.Freeze();
        copy.IsFrozen.ShouldBeTrue();
        copy.ShouldBe(orig);

        await Verifier.Verify(json);
    }
}
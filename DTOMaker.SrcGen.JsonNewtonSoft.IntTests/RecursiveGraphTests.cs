using DTOMaker.Models;
using DTOMaker.Runtime.JsonNewtonSoft;
using DTOMaker.SrcGen.JsonNewtonSoft.IntTests.JsonNewtonSoft;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonNewtonSoft.IntTests
{
    [Entity(30)]
    public interface INode : IEntityBase
    {
        [Member(1)][Name("key")] String Key { get; set; }
    }

    [Entity(31)]
    public interface IStringNode : INode
    {
        [Member(1)][Name("sVal")] String V { get; set; }
    }

    [Entity(32)]
    public interface INumberNode : INode
    {
        [Member(1)][Name("nVal")] Int64 V { get; set; }
    }

    [Entity(33)]
    public interface ITree : IEntityBase
    {
        [Member(1)][Name("left")] ITree? L { get; set; }
        [Member(2)][Name("right")] ITree? R { get; set; }
        [Member(3)][Name("node")] INode? N { get; set; }
    }

    public class RecursiveGraphTests
    {
        public string Roundtrip_Graph(Tree orig)
        {
            orig.Freeze();
            var json = orig.SerializeToJson();
            var copy = json.DeserializeFromJson<Tree>();
            copy.ShouldNotBeNull();
            copy.Freeze();
            copy.ShouldBe(orig);
            return json;
        }

        [Fact]
        public async Task Roundtrip_Tree()
        {
            var tree = new Tree
            {
                L = new Tree
                {
                    N = new StringNode
                    {
                        Key = "L",
                        V = "LStr"
                    }
                },
                R = new Tree
                {
                    N = new NumberNode
                    {
                        Key = "R",
                        V = 314L
                    }
                },
                N = new StringNode
                {
                    Key = "M",
                    V = "MStr"
                }
            };
            string json = Roundtrip_Graph(tree);
            await Verifier.Verify(json);
        }
    }
}

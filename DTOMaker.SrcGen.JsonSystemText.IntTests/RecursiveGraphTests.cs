using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.Runtime.JsonSystemText;
using DTOMaker.SrcGen.JsonSystemText.IntTests.JsonSystemText;
using Shouldly;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.JsonSystemText.IntTests
{
    [Entity]
    [Id(30)]
    public interface INode : IEntityBase
    {
        [Member(1)] String K { get; set; }
    }

    [Entity]
    [Id(31)]
    public interface IStringNode : INode
    {
        [Member(1)] String V { get; set; }
    }

    [Entity]
    [Id(32)]
    public interface INumberNode : INode
    {
        [Member(1)] Int64 V { get; set; }
    }

    [Entity]
    [Id(33)]
    public interface ITree : IEntityBase
    {
        [Member(1)] ITree? L { get; set; }
        [Member(2)] ITree? R { get; set; }
        [Member(3)] INode? N { get; set; }
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
                        K = "L",
                        V = "LStr"
                    }
                },
                R = new Tree
                {
                    N = new NumberNode
                    {
                        K = "R",
                        V = 314L
                    }
                },
                N = new StringNode
                {
                    K = "M",
                    V = "MStr"
                }
            };
            string json = Roundtrip_Graph(tree);
            await Verifier.Verify(json);
        }
    }
}

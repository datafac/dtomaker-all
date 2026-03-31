using DTOMaker.Models;
using DTOMaker.Runtime;
using DTOMaker.SrcGen.MemBlocks.IntTests.MemBlocks;
using Shouldly;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace DTOMaker.SrcGen.MemBlocks.IntTests
{
    [Entity(30)]
    public interface INode : IEntityBase
    {
        [Member(1)] String K { get; set; }
    }

    [Entity(31)]
    public interface IStringNode : INode
    {
        [Member(1)] String V { get; set; }
    }

    [Entity(32)]
    public interface INumberNode : INode
    {
        [Member(1)] Int64 V { get; set; }
    }

    [Entity(33)]
    public interface ITree : IEntityBase
    {
        [Member(1)] ITree? L { get; set; }
        [Member(2)] ITree? R { get; set; }
        [Member(3)] INode? N { get; set; }
    }

    public static class VerifyHelpers
    {
        private static IEnumerable<byte> ToBytes(this ReadOnlySequence<byte> buffer)
        {
            foreach (var segment in buffer)
            {
                for (var i = 0; i < segment.Length; i++)
                {
                    yield return segment.Span[i];
                }
            }
        }

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

    public class RecursiveGraphTests
    {
        public async Task<string> Roundtrip_GraphAsync(Tree orig)
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            await orig.Pack(dataStore);
            var buffer = orig.GetBuffer();
            var copy = new Tree(buffer);
            copy.ShouldNotBeNull();
            copy.Freeze();
            copy.ShouldBe(orig);
            return buffer.ToDisplay();
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
            string buffer = await Roundtrip_GraphAsync(tree);
            await Verifier.Verify(buffer);
        }
    }
}

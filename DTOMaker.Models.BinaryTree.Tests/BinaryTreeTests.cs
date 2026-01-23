using DTOMaker.Runtime.MemBlocks;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DTOMaker.Models.BinaryTree.Tests
{
    public class BinaryTreeTests
    {
        private static IBinaryTree<string, long> CreateEmpty(ImplKind kind)
        {
            return kind switch
            {
                ImplKind.JsonSystemText => new TestOrg.TestApp.Models.JsonSystemText.MyBinaryTree(),
                //ImplKind.MemBlocks => new nodeFactory_MemBlocks(),
                ImplKind.MsgPack2 => new TestOrg.TestApp.Models.MsgPack2.MyBinaryTree(),
                _ => throw new NotSupportedException($"Unsupported implementation kind: {kind}"),
            };
        }

        [Theory]
        [InlineData(ImplKind.JsonSystemText, "b", 1)]
        [InlineData(ImplKind.JsonSystemText, "ba", 2)]
        [InlineData(ImplKind.JsonSystemText, "bc", 2)]
        [InlineData(ImplKind.JsonSystemText, "abc", 2)]
        [InlineData(ImplKind.JsonSystemText, "acb", 3)]
        [InlineData(ImplKind.JsonSystemText, "bac", 2)]
        [InlineData(ImplKind.JsonSystemText, "bca", 2)]
        [InlineData(ImplKind.JsonSystemText, "cba", 2)]
        [InlineData(ImplKind.JsonSystemText, "cab", 3)]
        [InlineData(ImplKind.JsonSystemText, "dbacfeg", 3)]
        [InlineData(ImplKind.JsonSystemText, "abcdefg", 3)]
        [InlineData(ImplKind.MsgPack2, "abcdefg", 3)]
        //[InlineData(ImplKind.MemBlocks, "abcdefg", 3)]
        public void AddValues(ImplKind impl, string order, byte maxDepth)
        {
            // todo max depth tests

            using var dataStore = new DataFac.Storage.Testing.TestDataStore();

            IBinaryTree<string, long>? tree = null;

            // add nodes in order
            int count = 0;
            foreach (char ch in order)
            {
                long value = (Char.IsLetter(ch) && Char.IsLower(ch)) ? (ch - 'a') + 1 : throw new ArgumentException($"Unexpected character: {ch}");
                tree = tree.AddOrUpdate(new string(ch, 1), value, () => CreateEmpty(impl));
                count++;

                // pack and freeze the tree after each addition
                if (tree is IMemBlocksEntityBase packable) packable.Pack(dataStore);
                tree.Freeze();
            }

            // checks
            var pairs = tree.GetKeyValuePairs(false).ToArray();
            for (int i = 0; i < pairs.Length; i++)
            {
                if (i > 0)
                {
                    pairs[i].Key.ShouldBeGreaterThan(pairs[i - 1].Key);
                }
            }
            tree.ShouldNotBeNull();
            tree.Count.ShouldBe(count);
            tree.Depth.ShouldBeLessThanOrEqualTo(maxDepth);

            var node = tree.Get("b");
            node.ShouldNotBeNull();
            node.Key.ShouldBe("b");
            node.Value.ShouldBe(2L);

            node = tree.Get("z");
            node.ShouldBeNull();
        }

        [Theory]
        [InlineData(ImplKind.JsonSystemText, "b", "b", 0)]
        [InlineData(ImplKind.JsonSystemText, "bac", "a", 2)]
        [InlineData(ImplKind.JsonSystemText, "bac", "c", 2)]
        [InlineData(ImplKind.JsonSystemText, "bac", "ac", 1)]
        [InlineData(ImplKind.JsonSystemText, "bac", "ca", 1)]
        [InlineData(ImplKind.JsonSystemText, "bac", "acb", 0)]
        // perfect add/remove orders
        [InlineData(ImplKind.JsonSystemText, "dbfaceg", "", 3)]
        [InlineData(ImplKind.JsonSystemText, "dbfaceg", "a", 3)]
        [InlineData(ImplKind.JsonSystemText, "dbfaceg", "ac", 3)]
        [InlineData(ImplKind.JsonSystemText, "dbfaceg", "ace", 3)]
        [InlineData(ImplKind.JsonSystemText, "dbfaceg", "aceg", 2)]
        [InlineData(ImplKind.JsonSystemText, "dbfaceg", "acegb", 2)]
        [InlineData(ImplKind.JsonSystemText, "dbfaceg", "acegbf", 1)]
        [InlineData(ImplKind.JsonSystemText, "dbfaceg", "acegbfd", 0)]
        // other serializers
        [InlineData(ImplKind.MsgPack2, "dbfaceg", "acegbfd", 0)]
        public void RemoveValues(ImplKind impl, string addOrder, string removeOrder, byte maxDepth)
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();

            IBinaryTree<string, long>? tree = null;

            // add nodes in order
            int count = 0;
            foreach (char ch in addOrder)
            {
                long value = (Char.IsLetter(ch) && Char.IsLower(ch)) ? (ch - 'a') + 1 : throw new ArgumentException($"Unexpected character: {ch}");
                tree = tree.AddOrUpdate(new string(ch, 1), value, () => CreateEmpty(impl));
                count++;

                // pack and freeze the tree after each addition
                if (tree is IMemBlocksEntityBase packable) packable.Pack(dataStore);
                tree.Freeze();
            }

            // remove nodes in order
            foreach (char ch in removeOrder)
            {
                long value = (Char.IsLetter(ch) && Char.IsLower(ch)) ? (ch - 'a') + 1 : throw new ArgumentException($"Unexpected character: {ch}");
                tree = tree?.Remove(new string(ch, 1));
                count--;

                // pack and freeze the tree after each addition
                if (tree is IMemBlocksEntityBase packable) packable.Pack(dataStore);
                tree?.Freeze();
            }

            if (tree is null)
            {
                count.ShouldBe(0);
                maxDepth.ShouldBe((byte)0);
            }
            else
            {
                var pairs = tree.GetKeyValuePairs(false).ToArray();
                for (int i = 0; i < pairs.Length; i++)
                {
                    if (i > 0)
                    {
                        pairs[i].Key.ShouldBeGreaterThan(pairs[i - 1].Key);
                    }
                }
                tree.Count.ShouldBe(count);
                tree.Depth.ShouldBeLessThanOrEqualTo(maxDepth);
            }
        }

        private static IEnumerable<string> GetCharCombinations(string chars)
        {
            if (chars.Length == 1)
            {
                yield return new string(chars[0], 1);
            }
            else
            {
                for (int i = 0; i < chars.Length; i++)
                {
                    char ch = chars[i];
                    string remaining = chars.ToString().Remove(i, 1);
                    foreach (var subCombination in GetCharCombinations(remaining))
                    {
                        yield return ch + subCombination;
                    }
                }
            }
        }

        [Theory]
        [InlineData("ab", "ab,ba")]
        [InlineData("abc", "abc,acb,bac,bca,cab,cba")]
        [InlineData("abcd", "abcd,abdc,acbd,acdb,adbc,adcb,bacd,badc,bcad,bcda,bdac,bdca,cabd,cadb,cbad,cbda,cdab,cdba,dabc,dacb,dbac,dbca,dcab,dcba")]
        public void CheckCharCombinations(string input, string expected)
        {
            string[] combinations = GetCharCombinations(input).ToArray();
            string.Join(",", combinations).ShouldBe(expected);
        }

        //    [Theory]
        //    [InlineData("a", 1)]
        //    [InlineData("ab", 2)]
        //    [InlineData("abc", 3)]
        //    [InlineData("abcd", 3)]
        //    [InlineData("abcde", 4)]
        //    [InlineData("abcdef", 4)]
        //    [InlineData("abcdefg", 4)]
        //    public void AllCombinations(string chars, short maxDepth)
        //    {
        //        var nodeFactory = GetNodeFactory(ImplKind.CSPoco);
        //        using var dataStore = new DataFac.Storage.Testing.TestDataStore();

        //        foreach (string order in GetCharCombinations(chars))
        //        {
        //            var tree = CreateEmpty(impl);
        //            {
        //                if (tree is IPackable packable) packable.Pack(dataStore);
        //                tree.Freeze();
        //            }
        //            tree.Count.ShouldBe(0);
        //            tree.Depth.ShouldBe((short)0);

        //            // add nodes in order
        //            int count = 0;
        //            foreach (char ch in order)
        //            {
        //                long value = (Char.IsLetter(ch) && Char.IsLower(ch)) ? (ch - 'a') + 1 : throw new ArgumentException($"Unexpected character: {ch}");
        //                tree = tree.AddOrUpdate(new string(ch, 1), value, nodeFactory);
        //                count++;

        //                // pack and freeze the tree after each addition
        //                if (tree is IPackable packable) packable.Pack(dataStore);
        //                tree.Freeze();
        //            }


        //            var pairs = tree.GetKeyValuePairs().ToArray();
        //            for (int i = 0; i < pairs.Length; i++)
        //            {
        //                if (i > 0)
        //                {
        //                    pairs[i].Key.ShouldBeGreaterThan(pairs[i - 1].Key);
        //                }
        //            }
        //            tree.Count.ShouldBe(count);
        //            int depth = tree.Depth;
        //            depth.ShouldBeLessThanOrEqualTo(maxDepth, $"Input='{order}',depth={depth}");
        //        }
        //    }
    }
}
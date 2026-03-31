using DTOMaker.Runtime;
using DTOMaker.Runtime.MemBlocks;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using TestOrg.TestApp.Models.JsonSystemText;
using Xunit;

namespace DTOMaker.Models.BinaryTree.Tests
{
    public class BinaryTreeTests
    {
        [Theory]
        [InlineData("b", 1)]
        [InlineData("ba", 2)]
        [InlineData("bc", 2)]
        [InlineData("abc", 2)]
        [InlineData("acb", 3)]
        [InlineData("bac", 2)]
        [InlineData("bca", 2)]
        [InlineData("cba", 2)]
        [InlineData("cab", 3)]
        [InlineData("dbacfeg", 3)]
        [InlineData("abcdefg", 3)]
        //[InlineData(ImplKind.MsgPack2, "abcdefg", 3)]
        //[InlineData(ImplKind.MemBlocks, "abcdefg", 3)]
        public void AddValues(string order, byte maxDepth)
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            MyBinaryTree? tree = new MyBinaryTree();

            // add nodes in order
            int count = 0;
            foreach (char ch in order)
            {
                long value = (Char.IsLetter(ch) && Char.IsLower(ch)) ? (ch - 'a') + 1 : throw new ArgumentException($"Unexpected character: {ch}");
                tree = tree.AddOrUpdate(new string(ch, 1), value);
                count++;

                // pack and freeze the tree after each addition
                if (tree is IMemoryBlockEntity packable) packable.Pack(dataStore);
                tree.Freeze();
            }

            // checks
            KeyValuePair<string, long>[] pairs = tree.GetKeyValuePairs<string, long, TestOrg.TestApp.Models.JsonSystemText.MyBinaryTree>(false).ToArray();
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

            var node = tree.Get<string, long, MyBinaryTree>("b");
            node.ShouldNotBeNull();
            node.Key.ShouldBe("b");
            node.Value.ShouldBe(2L);

            node = tree.Get<string, long, MyBinaryTree>("z");
            node.ShouldBeNull();
        }

        [Theory]
        [InlineData("b", "b", 0)]
        [InlineData("bac", "a", 2)]
        [InlineData("bac", "c", 2)]
        [InlineData("bac", "ac", 1)]
        [InlineData("bac", "ca", 1)]
        [InlineData("bac", "acb", 0)]
        // perfect add/remove orders
        [InlineData("dbfaceg", "", 3)]
        [InlineData("dbfaceg", "a", 3)]
        [InlineData("dbfaceg", "ac", 3)]
        [InlineData("dbfaceg", "ace", 3)]
        [InlineData("dbfaceg", "aceg", 2)]
        [InlineData("dbfaceg", "acegb", 2)]
        [InlineData("dbfaceg", "acegbf", 1)]
        [InlineData("dbfaceg", "acegbfd", 0)]
        // other serializers
        //[InlineData(ImplKind.MsgPack2, "dbfaceg", "acegbfd", 0)]
        //[InlineData(ImplKind.MemBlocks, "dbfaceg", "acegbfd", 0)]
        public void RemoveValues(string addOrder, string removeOrder, byte maxDepth)
        {
            using var dataStore = new DataFac.Storage.Testing.TestDataStore();
            MyBinaryTree? tree = new MyBinaryTree();

            // add nodes in order
            int count = 0;
            foreach (char ch in addOrder)
            {
                long value = (Char.IsLetter(ch) && Char.IsLower(ch)) ? (ch - 'a') + 1 : throw new ArgumentException($"Unexpected character: {ch}");
                tree = tree.AddOrUpdate(new string(ch, 1), value);
                count++;

                // pack and freeze the tree after each addition
                if (tree is IMemoryBlockEntity packable) packable.Pack(dataStore);
                tree.Freeze();
            }

            // remove nodes in order
            foreach (char ch in removeOrder)
            {
                long value = (Char.IsLetter(ch) && Char.IsLower(ch)) ? (ch - 'a') + 1 : throw new ArgumentException($"Unexpected character: {ch}");
                tree = tree.Remove<string, long , MyBinaryTree>(new string(ch, 1));
                count--;

                // pack and freeze the tree after each addition
                if (tree is IMemoryBlockEntity packable) packable.Pack(dataStore);
                tree?.Freeze();
            }

            if (tree is null)
            {
                count.ShouldBe(0);
                maxDepth.ShouldBe((byte)0);
            }
            else
            {
                var pairs = tree.GetKeyValuePairs<string, long, MyBinaryTree>(false).ToArray();
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
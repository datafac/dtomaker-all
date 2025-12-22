//using DataFac.Memory;
//using DTOMaker.Models;
//using DTOMaker.Runtime;
//using DTOMaker.Runtime.MsgPack2;
//using DTOMaker.SrcGen.MsgPack2.IntTests.MsgPack2;
//using Shouldly;
//using System;
//using System.Threading.Tasks;
//using VerifyXunit;
//using Xunit;

//namespace DTOMaker.SrcGen.MsgPack2.IntTests;

//[Entity]
//[Id(18)]
//public interface ISimpleDTO_PairOfInt64 : IEntityBase
//{
//    [Member(1)] PairOfInt64 Field1 { get; set; }
//    [Member(2)] PairOfInt64? Field2 { get; set; }
//}

//public class RoundtripBasicTypeTests_PairOfInt64
//{
//    public ReadOnlyMemory<byte> Roundtrip_PairOfInt64(PairOfInt64 reqValue, PairOfInt64? optValue)
//    {
//        var orig = new SimpleDTO_PairOfInt64 { Field1 = reqValue, Field2 = optValue };
//        orig.Freeze();
//        orig.Field1.ShouldBe(reqValue);
//        orig.Field2.ShouldBe(optValue);
//        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
//        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_PairOfInt64>();
//        copy.ShouldNotBeNull();
//        copy.ShouldBe(orig);
//        copy.Field1.ShouldBe(reqValue);
//        return buffer;
//    }

//    [Fact] public async Task Roundtrip_PairOfInt64_Defaults() => await Verifier.Verify(Roundtrip_PairOfInt64(default, default));
//    [Fact]
//    public async Task Roundtrip_PairOfInt64_Maximums()
//        => await Verifier.Verify(Roundtrip_PairOfInt64(
//            new PairOfInt64(Int64.MaxValue, Int64.MaxValue),
//            new PairOfInt64(Int64.MinValue, Int64.MinValue)));
//}

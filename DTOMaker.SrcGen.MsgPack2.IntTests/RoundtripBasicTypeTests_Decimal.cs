//using DTOMaker.Models;
//using DTOMaker.Runtime.MsgPack2;
//using DTOMaker.SrcGen.MsgPack2.IntTests.MsgPack2;
//using Shouldly;
//using System;
//using System.Threading.Tasks;
//using VerifyXunit;
//using Xunit;

//namespace DTOMaker.SrcGen.MsgPack2.IntTests;

//[Entity(19)]
//public interface ISimpleDTO_Decimal : IEntityBase
//{
//    [Member(1)] Decimal Field1 { get; set; }
//    [Member(2)] Decimal? Field2 { get; set; }
//}

//public class RoundtripBasicTypeTests_Decimal
//{
//    public string Roundtrip_Decimal(Decimal reqValue, Decimal? optValue)
//    {
//        var orig = new SimpleDTO_Decimal { Field1 = reqValue, Field2 = optValue };
//        orig.Freeze();
//        orig.Field1.ShouldBe(reqValue);
//        orig.Field2.ShouldBe(optValue);
//        ReadOnlyMemory<byte> buffer = orig.SerializeToMessagePack();
//        var copy = buffer.DeserializeFromMessagePack<SimpleDTO_Decimal>();
//        copy.ShouldNotBeNull();
//        copy.ShouldBe(orig);
//        copy.Field1.ShouldBe(reqValue);
//        return buffer.Span.ToDisplay();
//    }

//    [Fact] public async Task Roundtrip_Decimal_Defaults() => await Verifier.Verify(Roundtrip_Decimal(default, default));
//    [Fact] public async Task Roundtrip_Decimal_Maximums() => await Verifier.Verify(Roundtrip_Decimal(Decimal.MaxValue, Decimal.MinValue));
//    [Fact] public async Task Roundtrip_Decimal_UnitVals() => await Verifier.Verify(Roundtrip_Decimal(Decimal.One, Decimal.MinusOne));

//}

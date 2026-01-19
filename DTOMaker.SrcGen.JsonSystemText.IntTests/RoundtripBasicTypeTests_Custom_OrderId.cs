using System;
using System.Threading.Tasks;
using DTOMaker.Models;
using DTOMaker.Runtime.JsonSystemText;
using Shouldly;
using VerifyXunit;
using Xunit;

using DTOMaker.SrcGen.JsonSystemText.IntTests.JsonSystemText;

namespace DTOMaker.SrcGen.JsonSystemText.IntTests;

public readonly struct OrderId : IEquatable<OrderId>
{
    public readonly long Value;
    public OrderId(long value) { Value = value; }
    #region IEquatable implementation
    public bool Equals(OrderId other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is OrderId other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Value);
    public static bool operator ==(OrderId left, OrderId right) => left.Equals(right);
    public static bool operator !=(OrderId left, OrderId right) => !left.Equals(right);
    #endregion
}

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="OrderId"/> properties to be internally 
/// stored and serialized as <see cref="System.Int64"/> values.
/// </summary>
public sealed class OrderIdConverter : IStructConverter<OrderId, long>
{
    public static OrderId ToCustom(long native) => new OrderId(native);
    public static OrderId? ToCustom(long? native) => native.HasValue ? new OrderId(native.Value) : null;
    public static long ToNative(OrderId custom) => custom.Value;
    public static long? ToNative(OrderId? custom) => custom.HasValue ? custom.Value.Value : null;
}
[Entity(43, LayoutMethod.Linear)]
public interface IOrderLine : IEntityBase
{
    [Member(1, NativeType.Int64, typeof(OrderIdConverter))] OrderId Id { get; set; }
}

public class RoundtripBasicTypeTests_Custom_OrderId
{
    public string Roundtrip_Complex(OrderId reqValue)
    {
        var orig = new OrderLine { Id = reqValue };
        orig.Freeze();
        orig.Id.ShouldBe(reqValue);
        var json = orig.SerializeToJson();
        var copy = json.DeserializeFromJson<OrderLine>();
        copy.ShouldNotBeNull();
        copy.ShouldBe(orig);
        copy.Id.ShouldBe(reqValue);
        return json;
    }

    [Fact] public async Task Roundtrip_Complex_Defaults() => await Verifier.Verify(Roundtrip_Complex(default));
    [Fact] public async Task Roundtrip_Complex_OneValue() => await Verifier.Verify(Roundtrip_Complex(new OrderId(1)));
    [Fact] public async Task Roundtrip_Complex_OthValue() => await Verifier.Verify(Roundtrip_Complex(new OrderId(long.MaxValue)));

}


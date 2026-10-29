using DataFac.Memory;
using System.Collections;
namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.BitArray"/> properties to be internally 
/// stored and serialized as <see cref="System.Octets"/> values.
/// </summary>
public sealed class BitArrayConverter : IObjectConverter<BitArray, Octets>
{
    public NativeType NativeType => NativeType.Binary;
    public static BitArray ToCustom(Octets native) => new BitArray(native.ToByteArray());
    public static Octets ToNative(BitArray custom)
    {
        var output = new byte[(custom.Length + 7) / 8];
        custom.CopyTo(output, 0);
        return Octets.Wrap(output);
    }
}


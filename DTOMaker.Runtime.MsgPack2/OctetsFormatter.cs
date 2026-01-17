using DataFac.Memory;
using MessagePack;
using MessagePack.Formatters;
using System.Buffers;

namespace DTOMaker.Runtime.MsgPack2
{
    internal sealed class OctetsFormatter : IMessagePackFormatter<Octets>
    {
        public Octets Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var seq = reader.ReadBytes();
            if (seq is null) return null!;
            if (seq.Value.Length == 0) return Octets.Empty;
            return Octets.UnsafeWrap(seq.Value);
        }
        public void Serialize(ref MessagePackWriter writer, Octets value, MessagePackSerializerOptions options)
        {
            if (value is not null)
                writer.Write(value.Sequence);
            else
                writer.WriteNil();
        }
    }
}

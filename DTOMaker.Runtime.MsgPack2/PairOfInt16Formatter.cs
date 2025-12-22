using DataFac.Memory;
using MessagePack;
using MessagePack.Formatters;

namespace DTOMaker.Runtime.MsgPack2
{
    internal sealed class PairOfInt16Formatter : IMessagePackFormatter<PairOfInt16>
    {
        public PairOfInt16 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                int count = reader.ReadArrayHeader();
                if (count != 2)
                {
                    throw new MessagePackSerializationException("Invalid array header");
                }
                options.Security.DepthStep(ref reader);
                try
                {
                    var a = reader.ReadInt16();
                    var b = reader.ReadInt16();
                    return new PairOfInt16(a, b);
                }
                finally
                {
                    reader.Depth--;
                }
            }
        }
        public void Serialize(ref MessagePackWriter writer, PairOfInt16 value, MessagePackSerializerOptions options)
        {
            if (value == default)
            {
                writer.WriteNil();
                return;
            }
            else
            {
                writer.WriteArrayHeader(2);
                writer.Write(value.A);
                writer.Write(value.B);
            }
        }
    }
}

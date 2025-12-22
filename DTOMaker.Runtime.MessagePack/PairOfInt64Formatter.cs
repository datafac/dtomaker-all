using DataFac.Memory;
using MessagePack;
using MessagePack.Formatters;

namespace DTOMaker.Runtime.MessagePack
{
    internal sealed class PairOfInt64Formatter : IMessagePackFormatter<PairOfInt64>
    {
        public PairOfInt64 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
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
                    var a = reader.ReadInt64();
                    var b = reader.ReadInt64();
                    return new PairOfInt64(a, b);
                }
                finally
                {
                    reader.Depth--;
                }
            }
        }
        public void Serialize(ref MessagePackWriter writer, PairOfInt64 value, MessagePackSerializerOptions options)
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

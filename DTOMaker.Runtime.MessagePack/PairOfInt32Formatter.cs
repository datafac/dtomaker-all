using DataFac.Memory;
using MessagePack;
using MessagePack.Formatters;

namespace DTOMaker.Runtime.MessagePack
{
    internal sealed class PairOfInt32Formatter : IMessagePackFormatter<PairOfInt32>
    {
        public PairOfInt32 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
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
                    var a = reader.ReadInt32();
                    var b = reader.ReadInt32();
                    return new PairOfInt32(a, b);
                }
                finally
                {
                    reader.Depth--;
                }
            }
        }
        public void Serialize(ref MessagePackWriter writer, PairOfInt32 value, MessagePackSerializerOptions options)
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

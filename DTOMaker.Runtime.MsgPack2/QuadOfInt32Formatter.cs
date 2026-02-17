using DataFac.Memory;
using MessagePack;
using MessagePack.Formatters;

namespace DTOMaker.Runtime.MsgPack2;

internal sealed class QuadOfInt32Formatter : IMessagePackFormatter<QuadOfInt32>
{
    public QuadOfInt32 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return default;
        }
        else
        {
            int count = reader.ReadArrayHeader();
            if (count != 4)
            {
                throw new MessagePackSerializationException("Invalid array header");
            }
            options.Security.DepthStep(ref reader);
            try
            {
                var a = reader.ReadInt32();
                var b = reader.ReadInt32();
                var c = reader.ReadInt32();
                var d = reader.ReadInt32();
                return new QuadOfInt32(a, b, c, d);
            }
            finally
            {
                reader.Depth--;
            }
        }
    }
    public void Serialize(ref MessagePackWriter writer, QuadOfInt32 value, MessagePackSerializerOptions options)
    {
        if (value == default)
        {
            writer.WriteNil();
            return;
        }
        else
        {
            writer.WriteArrayHeader(4);
            writer.Write(value.A);
            writer.Write(value.B);
            writer.Write(value.C);
            writer.Write(value.D);
        }
    }
}

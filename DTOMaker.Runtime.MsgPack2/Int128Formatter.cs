using DataFac.Memory;
using MessagePack;
using MessagePack.Formatters;
using System;

namespace DTOMaker.Runtime.MsgPack2
{
#if NET7_0_OR_GREATER
    internal sealed class Int128Formatter : IMessagePackFormatter<Int128>
    {
        public Int128 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            int count = reader.ReadArrayHeader();
            if (count != 2)
            {
                throw new MessagePackSerializationException("Invalid array header");
            }
            options.Security.DepthStep(ref reader);
            try
            {
                BlockB016 block = default;
                block.A.Int64ValueLE = reader.ReadInt64();
                block.B.Int64ValueLE = reader.ReadInt64();
                return block.Int128ValueLE;
            }
            finally
            {
                reader.Depth--;
            }
        }
        public void Serialize(ref MessagePackWriter writer, Int128 value, MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            BlockB016 block = default;
            block.Int128ValueLE = value;
            writer.Write(block.A.Int64ValueLE);
            writer.Write(block.B.Int64ValueLE);
        }
    }
#endif
}

using DataFac.Memory;
using MessagePack;
using MessagePack.Formatters;

namespace DTOMaker.Runtime.MessagePack
{
    internal sealed class CustomResolver : IFormatterResolver
    {
        public static readonly CustomResolver Instance = new CustomResolver();
        private CustomResolver() { }
        public IMessagePackFormatter<T>? GetFormatter<T>()
        {
            if (typeof(T) == typeof(PairOfInt64))
            {
                return new PairOfInt64Formatter() is IMessagePackFormatter<T> typedFormatter ? typedFormatter : null;
            }
            if (typeof(T) == typeof(PairOfInt32))
            {
                return new PairOfInt32Formatter() is IMessagePackFormatter<T> typedFormatter ? typedFormatter : null;
            }
            if (typeof(T) == typeof(PairOfInt16))
            {
                return new PairOfInt16Formatter() is IMessagePackFormatter<T> typedFormatter ? typedFormatter : null;
            }
            return null;
        }
    }
}

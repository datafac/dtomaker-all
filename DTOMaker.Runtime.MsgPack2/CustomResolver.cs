using DataFac.Memory;
using MessagePack;
using MessagePack.Formatters;
using System;

namespace DTOMaker.Runtime.MsgPack2
{
    internal sealed class CustomResolver : IFormatterResolver
    {
        public static readonly CustomResolver Instance = new CustomResolver();
        private CustomResolver() { }
        public IMessagePackFormatter<T>? GetFormatter<T>()
        {
#if NET7_0_OR_GREATER
            if (typeof(T) == typeof(Int128))
            {
                return new Int128Formatter() is IMessagePackFormatter<T> typedFormatter ? typedFormatter : null;
            }
            if (typeof(T) == typeof(UInt128))
            {
                return new UInt128Formatter() is IMessagePackFormatter<T> typedFormatter ? typedFormatter : null;
            }
#endif
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

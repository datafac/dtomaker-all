using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DTOMaker.SrcGen.Core
{
    public readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IReadOnlyCollection<T>
        where T : IEquatable<T>
    {
        private static readonly EquatableArray<T> _empty = new EquatableArray<T>();
        public static EquatableArray<T> Empty => _empty;

        private readonly ImmutableArray<T> _array;

        public EquatableArray() => _array = ImmutableArray<T>.Empty;
        public EquatableArray(IEnumerable<T> items) => _array = ImmutableArray.CreateRange(items);

        public int Count => _array.Length;
        public ImmutableArray<T> Array => _array;


        public bool Equals(EquatableArray<T> other) => _array.AsSpan().SequenceEqual(other.Array.AsSpan());
        public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);
        public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);
        public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !left.Equals(right);

        public override int GetHashCode()
        {
            HashCode hashCode = default;
            hashCode.Add(_array.Length);
            for (int i = 0; i < _array.Length; i++)
            {
                hashCode.Add(_array[i]);
            }
            return hashCode.ToHashCode();
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)_array).GetEnumerator();
    }

}
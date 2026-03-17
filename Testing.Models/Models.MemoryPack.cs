using DTOMaker.Models;
using System;
using MemoryPack;

namespace Testing.Models.MemPack
{
    [MemoryPackable]
    public partial class Required_Int64 : IRequired_Int64, IEquatable<Required_Int64>
    {
        public bool IsFrozen => false;
        public void Freeze() { }
        public IEntityBase PartCopy() => new Required_Int64() { Field = this.Field };
        [MemoryPackInclude] public long Field { get; set; }

        public bool Equals(Required_Int64? other) => other is not null;
        public override bool Equals(object? obj) => obj is Required_Int64 other && Equals(other);
        public override int GetHashCode()
        {
            HashCode hasher = new();
            hasher.Add(typeof(Required_Int64));
            hasher.Add(Field);
            return hasher.ToHashCode();
        }
    }
}

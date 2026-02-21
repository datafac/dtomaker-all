using System;
using System.Runtime.CompilerServices;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public class BinaryMap
{
    private readonly struct Result
    {
        public readonly bool Found;
        public readonly BinaryMap? NewMap;
        public readonly int Offset;
        public Result(bool found, BinaryMap? newMap, int offset)
        {
            Found = found;
            NewMap = newMap;
            Offset = offset;
        }
    }

    /// <summary>
    /// Default initial map contains 2^3 or 8 bits (1 byte)
    /// </summary>
    public static BinaryMap Empty { get; } = new BinaryMap(3, Fill.Zero, null, null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PowerOf2(sbyte x)
    {
        return 1 << x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Log2(int x)
    {
        sbyte result = 0;
        int test = 1;
        while (test < x)
        {
            result += 1;
            test *= 2;
        }
        return result;
    }

    private readonly sbyte _lenLog2;
    public Fill State { get; }

    public int SizeInBits { get; }
    public (int ByteCount, int BitsRemain) Sizes => (SizeInBits / 8, SizeInBits % 8);

    public readonly BinaryMap? A = null;
    public readonly BinaryMap? B = null;

    private BinaryMap(sbyte lenLog2, Fill fill, BinaryMap? a, BinaryMap? b)
    {
        if (lenLog2 < 0)
            throw new ArgumentOutOfRangeException(nameof(lenLog2), lenLog2, $"Must be >= 0");
        _lenLog2 = lenLog2;
        SizeInBits = PowerOf2(_lenLog2);
        A = a;
        B = b;
        State = fill;
    }

    public BinaryMap Expand()
    {
        var currentSizeInBytes = PowerOf2(_lenLog2) / 8;
        if (_lenLog2 >= 16)
            throw new InvalidOperationException($"Cannot expand block map beyond 8KiB (yet)");
        switch (State)
        {
            case Fill.Zero:
                return new BinaryMap((sbyte)(_lenLog2 + 1), Fill.Zero, null, null);
            case Fill.Part:
                return new BinaryMap((sbyte)(_lenLog2 + 1), Fill.Part, this, new BinaryMap(_lenLog2, Fill.Zero, null, null));
            case Fill.Full:
                return new BinaryMap((sbyte)(_lenLog2 + 1), Fill.Part, this, new BinaryMap(_lenLog2, Fill.Zero, null, null));
            default:
                throw new ArgumentOutOfRangeException(nameof(State), State, null);
        }
    }

    private static Fill CalcFill(Fill? a, Fill? b)
    {
        Fill ax = a ?? Fill.Zero;
        Fill bx = b ?? Fill.Zero;
        if (ax == Fill.Zero && bx == Fill.Zero)
            return Fill.Zero;
        if (ax == Fill.Full && bx == Fill.Full)
            return Fill.Full;
        return Fill.Part;
    }

    private Result assignField(sbyte bitlenLog2)
    {
        if (bitlenLog2 > _lenLog2)
            return new Result(false, null, 0);
        if (State == Fill.Full)
            return new Result(false, null, 0);
        if (bitlenLog2 == _lenLog2)
        {
            if (State == Fill.Zero)
                return new Result(true, new BinaryMap(_lenLog2, Fill.Full, null, null), 0);
            else
                return new Result(false, null, 0);
        }
        // check legs
        int half = 0;
        {
            var a = A ?? new BinaryMap((sbyte)(_lenLog2 - 1), Fill.Zero, null, null);
            var x = a.assignField(bitlenLog2);
            if (x.Found)
                return new Result(true, new BinaryMap(_lenLog2, CalcFill(x.NewMap?.State, B?.State), x.NewMap, B), x.Offset);
            half = a.SizeInBits;
        }
        {
            var b = B ?? new BinaryMap((sbyte)(_lenLog2 - 1), Fill.Zero, null, null);
            var x = b.assignField(bitlenLog2);
            if (x.Found)
                return new Result(true, new BinaryMap(_lenLog2, CalcFill(A?.State, x.NewMap?.State), A, x.NewMap), half + x.Offset);
        }
        return new Result(false, null, 0);
    }

    /// <summary>
    /// Finds and assigns a field of the given size (in bits).
    /// </summary>
    /// <param name="lengthInBits">The length of the field to assign (eg. byte = 8 bits, Int32 = 32 bits)</param>
    /// <returns>An updated map conatining the assignment.</returns>
    public (bool, BinaryMap, int) AssignField(int lengthInBits)
    {
        const int maxLengthInBits = 64 * 1024 * 8; // 64KiB * 8 bits
        if (lengthInBits <= 0)
            throw new ArgumentOutOfRangeException(nameof(lengthInBits), lengthInBits, $"Must be > 0");
        if (lengthInBits > maxLengthInBits)
            throw new InvalidOperationException($"Cannot add fields larger than 64KiB (yet)");
        sbyte lengthLog2 = Log2(lengthInBits);
        bool found = false;
        BinaryMap? map = this;
        int offset = 0;
        while (!found)
        {
            var x = map!.assignField(lengthLog2);
            found = x.Found;
            if (found)
            {
                map = x.NewMap;
                offset = x.Offset;
            }
            else
            {
                map = map.Expand();
            }
        }
        return (true, map!, offset);
    }
}

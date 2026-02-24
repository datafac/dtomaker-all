using DTOMaker.SrcGen.Core;
using System;
using System.Collections;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public record BlockMap
{
    public static bool Vacant(BitArray map, int position, int length)
    {
        for (int i = position; i < (position + length); i++)
        {
            if (map[i]) return false;
        }

        return true;
    }

    public static void Reserve(BitArray map, int position, int length)
    {
        for (int i = position; i < position + length; i++)
        {
            if (map[i]) throw new InvalidOperationException($"Cannot reserve map[{position}..{position + length - 1}] - map[{i}] is already reserved");
            map[i] = true;
        }
    }

    public int BlockSize { get; init; }
    public EquatableArray<FieldDef> Fields { get; init; } = EquatableArray<FieldDef>.Empty;

    public BlockMapBuilder ToBuilder()
    {
        var builder = new BlockMapBuilder();
        builder.AddFields(Fields);
        return builder;
    }

    private string? GetFirstValidationError()
    {
        var map = new BitArray(BlockSize);
        foreach (FieldDef field in Fields)
        {
            // check alignment
            if (field.Offset % field.Length != 0)
                return
                    $"Field incorrectly aligned (Offset={field.Offset},Length={field.Length})";

            // check memory overlaps
            var bytesNeeded = field.Length;
            if (Vacant(map, field.Offset, bytesNeeded))
            {
                Reserve(map, field.Offset, bytesNeeded);
            }
            else
            {
                return $"Field incorrectly mapped (Offset={field.Offset},Length={field.Length})";
            }
        }

        return null;
    }

    public bool IsValid(bool throwOnError = false)
    {
        string? reason = GetFirstValidationError();
        if (reason is null)
            return true;
        if (throwOnError)
            throw new InvalidOperationException(reason);
        return false;
    }

}

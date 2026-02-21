using DTOMaker.SrcGen.Core;
using System;
using System.Collections;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public record BlockMap
{
    public int BlockSize { get; init; }
    public EquatableArray<FieldDef> Fields { get; init; } = EquatableArray<FieldDef>.Empty;

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

    private string? GetFirstValidationError()
    {
        var map = new BitArray(BlockSize);
        foreach (FieldDef field in Fields)
        {
            // check alignment
            if (field.BlockOffset % field.FieldLength != 0)
                return
                    $"Field '{field.FieldName}' ({field.FieldType}) incorrectly aligned (BlockOffset={field.BlockOffset},FieldLength={field.FieldLength})";

            // check memory overlaps
            var bytesNeeded = field.FieldLength;
            if (Vacant(map, field.BlockOffset, bytesNeeded))
            {
                Reserve(map, field.BlockOffset, bytesNeeded);
            }
            else
            {
                return
                    $"Field '{field.FieldName}' ({field.FieldType}) incorrectly mapped (BlockOffset={field.BlockOffset},FieldLength={field.FieldLength})";
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

using DTOMaker.SrcGen.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public class BlockMapBuilder
{
    private BinaryMap _map = BinaryMap.Empty;
    private List<FieldDef> _fields = new List<FieldDef>();

    public BlockMapBuilder() { }

    public BlockMapBuilder AddField(FieldDef fieldDef)
    {
        int lengthInBits = fieldDef.Length * 8;
        (BinaryMap newMap, int bitOffset) = _map.AssignField(lengthInBits);
        var bitsRemain = bitOffset % 8;
        if (bitsRemain != 0)
            throw new InvalidOperationException($"Can't do bit offsets yet");
        int byteOffset = bitOffset / 8;
        _map = newMap;
        _fields.Add(fieldDef with { Offset = byteOffset });
        return this;
    }

    public BlockMapBuilder AddFields(IEnumerable<FieldDef> fields)
    {
        foreach (var field in fields)
        {
            AddField(field);
        }
        return this;
    }

    public BlockMap Build()
    {
        if (_map.Sizes.BitsRemain != 0)
            throw new InvalidOperationException($"Cannot build maps with bit remainders (yet)");

        return _map.State == Fill.Zero
            ? new BlockMap() { BlockSize = 0 }
            : new BlockMap() { BlockSize = _map.Sizes.ByteCount, Fields = new EquatableArray<FieldDef>(_fields) };
    }
}

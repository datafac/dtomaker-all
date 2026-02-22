using DTOMaker.SrcGen.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOMaker.SrcGen.MemBlocks.BlockLayout;

public class BlockMapBuilder
{
    private BinaryMap _map = BinaryMap.Empty;
    private List<FieldDef> _fields = new List<FieldDef>();

    public BlockMapBuilder(BlockMap? initialMap)
    {
        if (initialMap is null) return;
        foreach (FieldDef fd in initialMap.Fields)
        {
            AddField(fd.Sequence, fd.Name!, fd.Length, fd.IsFlagsByte);
        }
    }

    private void AddField(int sequence, string fieldName, int fieldLength, bool isFlagsByte)
    {
        int lengthInBits = fieldLength * 8;
        (bool found, BinaryMap newMap, int bitOffset) = _map.AssignField(lengthInBits);
        if (found)
        {
            var bitsRemain = bitOffset % 8;
            if (bitsRemain != 0)
                throw new InvalidOperationException($"Can't do bit offsets yet");
            int byteOffset = bitOffset / 8;
            _map = newMap;
            _fields.Add(new FieldDef(sequence, fieldName, byteOffset, fieldLength, isFlagsByte));
        }
        else
        {
            throw new InvalidOperationException($"Cannot add field '{fieldName}' (Length={fieldLength})");
        }
    }

    private static int GetFieldLength(MemberKind kind, NativeType fieldType)
    {
        return kind switch
        {
            MemberKind.Entity or MemberKind.String or MemberKind.Binary => 64,
            MemberKind.Struct => fieldType switch
            {
                NativeType.Boolean 
                    or NativeType.Byte 
                    or NativeType.SByte => 1,
                NativeType.Int16 
                    or NativeType.UInt16 
                    or NativeType.Char 
                    or NativeType.Half => 2,
                NativeType.Int32 
                    or NativeType.UInt32 
                    or NativeType.PairOfInt16 
                    or NativeType.Single => 4,
                NativeType.Int64 
                    or NativeType.UInt64 
                    or NativeType.PairOfInt32 
                    or NativeType.Double => 8,
                NativeType.Decimal 
                    or NativeType.Int128 
                    or NativeType.UInt128 
                    or NativeType.PairOfInt64 
                    or NativeType.QuadOfInt32 
                    or NativeType.Guid => 16,
                NativeType.String 
                    or NativeType.Binary => 64,
                _ => 0,
            },
            _ => 0,
        };
    }

    public BlockMapBuilder AddRequests(IEnumerable<BlockMapRequest> requests)
    {
        foreach (var request in requests)
        {
            int fieldLength = GetFieldLength(request.MemberKind, request.NativeType);
            if (fieldLength > 0)
            {
                AddField(request.Sequence, request.FieldName!, fieldLength, request.IsFlagsByte);
            }
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

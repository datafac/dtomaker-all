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
            AddField(fd.FieldName!, fd.NativeType, fd.Nullable, fd.FieldLength, fd.BigEndian);
        }
    }

    private void AddField(string fieldName, NativeType fieldType, bool nullable, int fieldLength, bool bigEndian)
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
            _fields.Add(new FieldDef()
            {
                BigEndian = bigEndian,
                BlockOffset = byteOffset,
                FieldLength = fieldLength,
                FieldName = fieldName,
                NativeType = fieldType,
                Nullable = nullable,
            });
        }
        else
        {
            throw new InvalidOperationException($"Cannot add: {fieldType}({fieldLength}) {fieldName}; ");
        }
    }

    private static int GetFieldLength(NativeType fieldType)
    {
        switch (fieldType)
        {
            case NativeType.Boolean:
            case NativeType.Byte:
            case NativeType.SByte:
                return 1;

            case NativeType.Int16:
            case NativeType.UInt16:
            case NativeType.Char:
            case NativeType.Half:
                return 2;

            case NativeType.Int32:
            case NativeType.UInt32:
            case NativeType.PairOfInt16:
            case NativeType.Single:
                return 4;

            case NativeType.Int64:
            case NativeType.UInt64:
            case NativeType.PairOfInt32:
            case NativeType.Double:
                return 8;

            case NativeType.Decimal:
            case NativeType.Int128:
            case NativeType.UInt128:
            case NativeType.PairOfInt64:
            case NativeType.QuadOfInt32:
            case NativeType.Guid:
            //case NativeType.RawB10:
                return 16;

            //case NativeType.RawB20:
            //    return 32;

            case NativeType.String:
            case NativeType.Binary:
            //case NativeType.RawB40:
                return 64;

            //case NativeType.RawB80:
            //    return 128;

            default:
                throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, $"{nameof(NativeType)}({fieldType})");
        }
    }

    public BlockMapBuilder AddCommands(IEnumerable<FieldCommand> commands)
    {
        foreach (FieldCommand command in commands)
        {
            switch (command)
            {
                case AddFieldCommand addCommand:
                    {
                        int fieldLength = GetFieldLength(addCommand.NativeType);
                        AddField(addCommand.FieldName!, addCommand.NativeType, addCommand.Nullable, fieldLength, addCommand.BigEndian);
                        break;
                    }
                default:
                    throw new InvalidOperationException($"Unknown FieldCommand: {command}");
            }
        }
        return this;
    }

    public BlockMap Build()
    {
        var sizes = _map.Sizes;
        if (sizes.BitsRemain != 0)
            throw new InvalidOperationException($"Cannot build maps with bit remainders (yet)");
        return new BlockMap
        {
            BlockSize = sizes.ByteCount,
            Fields = new EquatableArray<FieldDef>(_fields),
        };
    }

}

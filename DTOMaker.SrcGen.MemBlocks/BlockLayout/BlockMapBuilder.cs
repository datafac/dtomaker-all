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
            AddField(fd.FieldName!, fd.FieldType, fd.Nullable, fd.FieldLength, fd.BigEndian);
        }
    }

    private void AddField(string fieldName, FieldType fieldType, bool nullable, int fieldLength, bool bigEndian)
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
                FieldType = fieldType,
                Nullable = nullable,
            });
        }
        else
        {
            throw new InvalidOperationException($"Cannot add: {fieldType}({fieldLength}) {fieldName}; ");
        }
    }

    private static int GetFieldLength(FieldType fieldType)
    {
        switch (fieldType)
        {
            case FieldType.Bool01:
            case FieldType.UInt01:
            case FieldType.SInt01:
                return 1;

            case FieldType.SInt02:
            case FieldType.UInt02:
            case FieldType.Char02:
            case FieldType.Real02:
                return 2;

            case FieldType.SInt04:
            case FieldType.UInt04:
            case FieldType.Real04:
                return 4;

            case FieldType.SInt08:
            case FieldType.UInt08:
            case FieldType.Real08:
                return 8;

            case FieldType.Deci10:
            case FieldType.SInt10:
            case FieldType.UInt10:
            case FieldType.Guid10:
            case FieldType.RawB10:
                return 16;

            case FieldType.RawB20:
                return 32;

            case FieldType.String:
            case FieldType.Binary:
            case FieldType.RawB40:
                return 64;

            case FieldType.RawB80:
                return 128;

            default:
                throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, $"{nameof(FieldType)}({fieldType})");
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
                        int fieldLength = GetFieldLength(addCommand.FieldType);
                        AddField(addCommand.FieldName!, addCommand.FieldType, addCommand.Nullable, fieldLength, addCommand.BigEndian);
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

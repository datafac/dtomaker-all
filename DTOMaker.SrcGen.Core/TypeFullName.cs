using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace DTOMaker.SrcGen.Core
{
    public readonly struct TypeFullName : IEquatable<TypeFullName>
    {
        public readonly ParsedName Intf;
        public readonly ParsedName Impl;

        private readonly ImmutableArray<ITypeParameterSymbol> _typeParameters; // generics only
        private readonly ImmutableArray<ITypeSymbol> _typeArguments; // closed generics only
        private readonly string _fullName;
        private readonly int _syntheticId;
        private readonly MemberKind _memberKind;
        private readonly bool _isCustom;
        private readonly NativeType _nativeType;

        private readonly string _implSpaceSuffix;

        private static string GetFullName(NativeType nativeType)
        {
            return nativeType switch
            {
                NativeType.Boolean => KnownType.SystemBoolean,
                NativeType.SByte => KnownType.SystemSByte,
                NativeType.Byte => KnownType.SystemByte,
                NativeType.Int16 => KnownType.SystemInt16,
                NativeType.UInt16 => KnownType.SystemUInt16,
                NativeType.Char => KnownType.SystemChar,
                NativeType.Half => KnownType.SystemHalf,
                NativeType.Int32 => KnownType.SystemInt32,
                NativeType.UInt32 => KnownType.SystemUInt32,
                NativeType.Single => KnownType.SystemSingle,
                NativeType.Int64 => KnownType.SystemInt64,
                NativeType.UInt64 => KnownType.SystemUInt64,
                NativeType.Double => KnownType.SystemDouble,
                NativeType.Int128 => KnownType.SystemInt128,
                NativeType.UInt128 => KnownType.SystemUInt128,
                NativeType.Guid => KnownType.SystemGuid,
                NativeType.Decimal => KnownType.SystemDecimal,
                NativeType.String => KnownType.SystemString,
                // custom types
                NativeType.PairOfInt16 => KnownType.PairOfInt16,
                NativeType.PairOfInt32 => KnownType.PairOfInt32,
                NativeType.PairOfInt64 => KnownType.PairOfInt64,
                NativeType.Binary => KnownType.MemoryOctets,
                _ => "System.Undefined",
            };
        }

        private static (MemberKind, NativeType) GetMemberKind(string fullname)
        {
            return fullname switch
            {
                KnownType.SystemBoolean => (MemberKind.Struct, NativeType.Boolean),
                KnownType.SystemSByte => (MemberKind.Struct, NativeType.SByte),
                KnownType.SystemByte => (MemberKind.Struct, NativeType.Byte),
                KnownType.SystemInt16 => (MemberKind.Struct, NativeType.Int16),
                KnownType.SystemUInt16 => (MemberKind.Struct, NativeType.UInt16),
                KnownType.SystemChar => (MemberKind.Struct, NativeType.Char),
                KnownType.SystemHalf => (MemberKind.Struct, NativeType.Half),
                KnownType.SystemInt32 => (MemberKind.Struct, NativeType.Int32),
                KnownType.SystemUInt32 => (MemberKind.Struct, NativeType.UInt32),
                KnownType.SystemSingle => (MemberKind.Struct, NativeType.Single),
                KnownType.SystemInt64 => (MemberKind.Struct, NativeType.Int64),
                KnownType.SystemUInt64 => (MemberKind.Struct, NativeType.UInt64),
                KnownType.SystemDouble => (MemberKind.Struct, NativeType.Double),
                KnownType.SystemInt128 => (MemberKind.Struct, NativeType.Int128),
                KnownType.SystemUInt128 => (MemberKind.Struct, NativeType.UInt128),
                KnownType.SystemGuid => (MemberKind.Struct, NativeType.Guid),
                KnownType.SystemDecimal => (MemberKind.Struct, NativeType.Decimal),
                KnownType.SystemString => (MemberKind.String, NativeType.String),
                // custom types
                KnownType.PairOfInt16 => (MemberKind.Struct, NativeType.PairOfInt16),
                KnownType.PairOfInt32 => (MemberKind.Struct, NativeType.PairOfInt32),
                KnownType.PairOfInt64 => (MemberKind.Struct, NativeType.PairOfInt64),
                KnownType.MemoryOctets => (MemberKind.Binary, NativeType.Binary),
                _ => (MemberKind.Undefined, NativeType.Undefined),
            };
        }

        private static int GetSyntheticId(NativeType nativeType)
        {
            return nativeType switch
            {
                NativeType.Boolean => 9001,
                NativeType.SByte => 9002,
                NativeType.Byte => 9003,
                NativeType.Int16 => 9004,
                NativeType.UInt16 => 9005,
                NativeType.Char => 9006,
                NativeType.Half => 9007,
                NativeType.Int32 => 9008,
                NativeType.UInt32 => 9009,
                NativeType.Single => 9010,
                NativeType.Int64 => 9011,
                NativeType.UInt64 => 9012,
                NativeType.Double => 9013,
                NativeType.Int128 => 9014,
                NativeType.UInt128 => 9015,
                NativeType.Guid => 9016,
                NativeType.Decimal => 9017,
                NativeType.String => 9018,
                // custom types
                NativeType.PairOfInt16 => 9096,
                NativeType.PairOfInt32 => 9097,
                NativeType.PairOfInt64 => 9098,
                NativeType.Binary => 9099,
                _ => 0,
            };
        }

        private TypeFullName(ParsedName intf, ParsedName impl, MemberKind kind, bool isCustom, NativeType nativeType, int syntheticId, ImmutableArray<ITypeParameterSymbol> typeParameters, ImmutableArray<ITypeSymbol> typeArguments, string implSpaceSuffix)
        {
            Intf = intf;
            Impl = impl;
            _typeParameters = typeParameters;
            _typeArguments = typeArguments;
            _fullName = Impl.Space + "." + MakeCSImplName(Impl.Name, typeParameters, typeArguments, implSpaceSuffix);
            _syntheticId = syntheticId;
            _memberKind = kind;
            _isCustom = isCustom;
            _nativeType = nativeType;
            _implSpaceSuffix = implSpaceSuffix;
        }

        public TypeFullName(ITypeSymbol ids, string implSpaceSuffix)
        {
            string nameSpace = ids.ContainingNamespace.ToDisplayString();
            Intf = new ParsedName(nameSpace, ids.Name);
            Impl = ids.TypeKind == TypeKind.Interface && ids.Name.StartsWith("I") ? new ParsedName(nameSpace + "." + implSpaceSuffix, ids.Name.Substring(1)) : Intf;
            _typeParameters = ids is INamedTypeSymbol nts1 ? nts1.TypeParameters : ImmutableArray<ITypeParameterSymbol>.Empty;
            _typeArguments = ids is INamedTypeSymbol nts2 ? nts2.TypeArguments : ImmutableArray<ITypeSymbol>.Empty;
            _fullName = Impl.Space + "." + MakeCSImplName(Impl.Name, _typeParameters, _typeArguments, implSpaceSuffix);
            (_memberKind, _nativeType) = GetMemberKind(_fullName);
            _isCustom = false;
            _syntheticId = GetSyntheticId(_nativeType);
            if (_memberKind == MemberKind.Undefined && ids.TypeKind == TypeKind.Interface)
            {
                _memberKind = MemberKind.Entity;
            }
            _implSpaceSuffix = implSpaceSuffix;
        }

        /// <summary>
        /// Special constructor to create a TypeFullName for a custom type member 
        /// </summary>
        /// <param name="tfn"></param>
        /// <param name="memberKind"></param>
        /// <param name="nativeType"></param>
        public TypeFullName(TypeFullName tfn, MemberKind memberKind, bool isCustom, NativeType nativeType)
        {
            _memberKind = memberKind;
            _isCustom = isCustom;
            _nativeType = nativeType;
            _fullName = GetFullName(nativeType);
            Impl = new ParsedName(_fullName);
            Intf = tfn.Intf;
            _typeParameters = ImmutableArray<ITypeParameterSymbol>.Empty;
            _typeArguments = ImmutableArray<ITypeSymbol>.Empty;
            _syntheticId = GetSyntheticId(nativeType);
            _implSpaceSuffix = tfn._implSpaceSuffix;
            //_fullName = Impl.Space + "." + MakeCSImplName(Impl.Name, _typeParameters, _typeArguments, _implSpaceSuffix);
        }

        public string IntfNameSpace => Intf.Space;
        public string ImplNameSpace => Impl.Space;
        public string ShortImplName => MakeCSImplName(Impl.Name, _typeParameters, _typeArguments, _implSpaceSuffix);
        public string ShortIntfName => MakeCSIntfName(Intf.Name, _typeParameters, _typeArguments);
        public string FullName => _fullName;
        public int SyntheticId => _syntheticId;
        public MemberKind MemberKind => _memberKind;
        public bool IsCustom => _isCustom;
        public NativeType NativeType => _nativeType;
        public bool IsGeneric => _typeParameters.Length > 0;
        public bool IsClosed => (_typeArguments.Length == _typeParameters.Length)
                                && _typeArguments.All(ta => ta.Kind != SymbolKind.TypeParameter);

        public ImmutableArray<ITypeParameterSymbol> TypeParameters => _typeParameters;
        public ImmutableArray<ITypeSymbol> TypeArguments => _typeArguments;
        public TypeFullName AsOpenGeneric()
        {
            return new TypeFullName(Intf, Impl, _memberKind, _isCustom, _nativeType, _syntheticId, _typeParameters, ImmutableArray<ITypeSymbol>.Empty, _implSpaceSuffix);
        }
        public TypeFullName AsClosedGeneric(ImmutableArray<ITypeSymbol> typeArguments)
        {
            return new TypeFullName(Intf, Impl, _memberKind, _isCustom, _nativeType, _syntheticId, _typeParameters, typeArguments, _implSpaceSuffix);
        }

        public bool Equals(TypeFullName other) => string.Equals(_fullName, other._fullName, StringComparison.Ordinal);
        public override bool Equals(object? obj) => obj is TypeFullName other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_fullName);
        public static bool operator ==(TypeFullName left, TypeFullName right) => left.Equals(right);
        public static bool operator !=(TypeFullName left, TypeFullName right) => !left.Equals(right);
        public override string ToString()
        {
            string intf = (Intf.Space == Impl.Space) ? Intf.Name : Intf.FullName;
            return $"{Impl.FullName} : {intf}";
        }

        /// <summary>
        /// Creates a unique entity name with closed generic arguments
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeParameters"></param>
        /// <param name="typeArguments"></param>
        /// <returns></returns>
        private static string MakeCSImplName(string name, ImmutableArray<ITypeParameterSymbol> typeParameters, ImmutableArray<ITypeSymbol> typeArguments, string implSpaceSuffix)
        {
            if (typeParameters.Length == 0) return name;

            StringBuilder result = new StringBuilder();
            result.Append(name);
            result.Append('_');
            result.Append(typeParameters.Length);
            for (int i = 0; i < typeParameters.Length; i++)
            {
                result.Append('_');
                if (i < typeArguments.Length && typeArguments[i].Kind == SymbolKind.NamedType)
                {
                    var aTFN = new TypeFullName(typeArguments[i], implSpaceSuffix);
                    result.Append(aTFN.ShortImplName);
                }
                else
                {
                    //var tp = typeParameters[i];
                    result.Append($"T{i}");
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Reconstructs the open or closed CSharp interface name. This should be the same as that given in the source model.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeParameters"></param>
        /// <param name="typeArguments"></param>
        /// <returns></returns>
        private static string MakeCSIntfName(string name, ImmutableArray<ITypeParameterSymbol> typeParameters, ImmutableArray<ITypeSymbol> typeArguments)
        {
            if (typeParameters.Length == 0) return name;

            StringBuilder result = new StringBuilder();
            result.Append(name);
            result.Append('<');
            for (int i = 0; i < typeParameters.Length; i++)
            {
                if (i > 0) result.Append(", ");
                if (i < typeArguments.Length)
                {
                    var ta = typeArguments[i];
                    result.Append(ta.Name);
                }
                else
                {
                    var tp = typeParameters[i];
                    result.Append(tp.Name);
                }
            }
            result.Append('>');
            return result.ToString();
        }
    }
}
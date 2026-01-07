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
        private readonly NativeType _nativeType;

        private readonly string _implSpaceSuffix;

        /// <summary>
        /// Determines the kind, native type, and associated synthetic id for a specified fully qualified type name.
        /// </summary>
        /// <param name="fullname">The fully qualified name of the type to evaluate. This value is typically a string representing a known
        /// system or custom type.</param>
        private static (MemberKind, NativeType, int) GetMemberKind(string fullname)
        {
            return fullname switch
            {
                KnownType.SystemBoolean => (MemberKind.Native, NativeType.Boolean, 9001),
                KnownType.SystemSByte => (MemberKind.Native, NativeType.SByte, 9002),
                KnownType.SystemByte => (MemberKind.Native, NativeType.Byte, 9003),
                KnownType.SystemInt16 => (MemberKind.Native, NativeType.Int16, 9004),
                KnownType.SystemUInt16 => (MemberKind.Native, NativeType.UInt16, 9005),
                KnownType.SystemChar => (MemberKind.Native, NativeType.Char, 9006),
                KnownType.SystemHalf => (MemberKind.Native, NativeType.Half, 9007),
                KnownType.SystemInt32 => (MemberKind.Native, NativeType.Int32, 9008),
                KnownType.SystemUInt32 => (MemberKind.Native, NativeType.UInt32, 9009),
                KnownType.SystemSingle => (MemberKind.Native, NativeType.Single, 9010),
                KnownType.SystemInt64 => (MemberKind.Native, NativeType.Int64, 9011),
                KnownType.SystemUInt64 => (MemberKind.Native, NativeType.UInt64, 9012),
                KnownType.SystemDouble => (MemberKind.Native, NativeType.Double, 9013),
                KnownType.SystemInt128 => (MemberKind.Native, NativeType.Int128, 9014),
                KnownType.SystemUInt128 => (MemberKind.Native, NativeType.UInt128, 9015),
                KnownType.SystemGuid => (MemberKind.Native, NativeType.Guid, 9016),
                KnownType.SystemDecimal => (MemberKind.Native, NativeType.Decimal, 9017),
                KnownType.SystemString => (MemberKind.String, NativeType.String, 9018),
                // custom types
                KnownType.PairOfInt16 => (MemberKind.Native, NativeType.PairOfInt16, 9096),
                KnownType.PairOfInt32 => (MemberKind.Native, NativeType.PairOfInt32, 9097),
                KnownType.PairOfInt64 => (MemberKind.Native, NativeType.PairOfInt64, 9098),
                KnownType.MemoryOctets => (MemberKind.Binary, NativeType.Binary, 9099),
                _ => (MemberKind.Undefined, NativeType.Undefined, 0),
            };
        }

        private TypeFullName(ParsedName intf, ParsedName impl, MemberKind kind, NativeType nativeType, int syntheticId, ImmutableArray<ITypeParameterSymbol> typeParameters, ImmutableArray<ITypeSymbol> typeArguments, string implSpaceSuffix)
        {
            Intf = intf;
            Impl = impl;
            _typeParameters = typeParameters;
            _typeArguments = typeArguments;
            _fullName = Impl.Space + "." + MakeCSImplName(Impl.Name, typeParameters, typeArguments, implSpaceSuffix);
            _syntheticId = syntheticId;
            _memberKind = kind;
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
            (_memberKind, _nativeType, _syntheticId) = GetMemberKind(_fullName);
            if (_memberKind == MemberKind.Undefined && ids.TypeKind == TypeKind.Interface)
            {
                _memberKind = MemberKind.Entity;
            }
            _implSpaceSuffix = implSpaceSuffix;
        }

        public string IntfNameSpace => Intf.Space;
        public string ImplNameSpace => Impl.Space;
        public string ShortImplName => MakeCSImplName(Impl.Name, _typeParameters, _typeArguments, _implSpaceSuffix);
        public string ShortIntfName => MakeCSIntfName(Intf.Name, _typeParameters, _typeArguments);
        public string FullName => _fullName;
        public int SyntheticId => _syntheticId;
        public MemberKind MemberKind => _memberKind;
        public NativeType NativeType => _nativeType;
        public bool IsGeneric => _typeParameters.Length > 0;
        public bool IsClosed => (_typeArguments.Length == _typeParameters.Length)
                                && _typeArguments.All(ta => ta.Kind != SymbolKind.TypeParameter);

        public ImmutableArray<ITypeParameterSymbol> TypeParameters => _typeParameters;
        public ImmutableArray<ITypeSymbol> TypeArguments => _typeArguments;
        public TypeFullName AsOpenGeneric()
        {
            return new TypeFullName(Intf, Impl, _memberKind, _nativeType, _syntheticId, _typeParameters, ImmutableArray<ITypeSymbol>.Empty, _implSpaceSuffix);
        }
        public TypeFullName AsClosedGeneric(ImmutableArray<ITypeSymbol> typeArguments)
        {
            return new TypeFullName(Intf, Impl, _memberKind, _nativeType, _syntheticId, _typeParameters, typeArguments, _implSpaceSuffix);
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
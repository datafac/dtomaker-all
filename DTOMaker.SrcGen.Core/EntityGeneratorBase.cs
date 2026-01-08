using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DTOMaker.SrcGen.Core
{
    public abstract class EntityGeneratorBase
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly TokenStack _tokenStack = new TokenStack();

        private readonly SourceGeneratorParameters _parameters;

        public EntityGeneratorBase(SourceGeneratorParameters parameters)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        private string ReplaceTokens(string input)
        {
            ILanguage language = _parameters.Language;

            // note token recursion not supported
            var tokenPrefix = language.TokenPrefix.AsSpan();
            var tokenSuffix = language.TokenSuffix.AsSpan();

            ReadOnlySpan<char> inputSpan = input.AsSpan();

            // fast exit for lines with no tokens
            if (inputSpan.IndexOf(tokenPrefix) < 0) return input;

            StringBuilder result = new StringBuilder();
            bool replaced = false;
            int remainderPos = 0;
            do
            {
                ReadOnlySpan<char> remainder = inputSpan.Slice(remainderPos);
                int tokenPos = remainder.IndexOf(tokenPrefix);
                int tokenEnd = tokenPos < 0 ? -1 : remainder.Slice(tokenPos + tokenPrefix.Length).IndexOf(tokenSuffix);
                if (tokenPos >= 0 && tokenEnd >= 0)
                {
                    // token found!
                    var tokenSpan = remainder.Slice(tokenPos + tokenPrefix.Length, tokenEnd);
                    string tokenName = tokenSpan.ToString();
                    if (_tokenStack.Top.TryGetValue(tokenName, out var tokenValue))
                    {
                        // replace valid token
                        // - emit prefix
                        // - emit token
                        // - calc remainder
                        ReadOnlySpan<char> prefix = remainder.Slice(0, tokenPos);
                        result.Append(prefix.ToString());
                        result.Append(language.GetValueAsCode(tokenValue));
                        remainderPos += (tokenPos + tokenPrefix.Length + tokenEnd + tokenSuffix.Length);
                        replaced = true;
                    }
                    else
                    {
                        // invalid token - emit error then original line
                        result.Clear();
                        result.AppendLine($"#error The token '{language.TokenPrefix}{tokenName}{language.TokenSuffix}' on the following line is invalid.");
                        result.AppendLine(input);
                        return result.ToString();
                    }
                }
                else
                {
                    // no token - emit remainder and return
                    result.Append(remainder.ToString());
                    return result.ToString();
                }
            }
            while (replaced);

            return result.ToString();
        }

        protected void Emit(string line)
        {
            _builder.AppendLine(ReplaceTokens(line));
        }

        private static string ToCamelCase(string value)
        {
            ReadOnlySpan<char> input = value.AsSpan();
            Span<char> output = stackalloc char[input.Length];
            input.CopyTo(output);
            for (int i = 0; i < output.Length; i++)
            {
                if (Char.IsLetter(output[i]))
                {
                    output[i] = Char.ToLower(output[i]);
                    return new string(output.ToArray());
                }
            }
            return new string(output.ToArray());
        }

        protected virtual string OnBuildTokenName(OutputMember member, string name)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(member.IsNullable ? "Nullable" : "Required");
            if (member.Kind == MemberKind.Struct)
            {
                sb.Append(member.IsCustom ? "Custom" : "Native");
            }
            sb.Append(member.Kind switch
            {
                MemberKind.Struct => "Struct",
                MemberKind.String => "String",
                MemberKind.Binary => "Binary",
                MemberKind.Entity => "Entity",
                _ => "NoKind"
            });
            sb.Append(name);
            return sb.ToString();
        }
        private string BuildTokenName(OutputMember member, string name) => OnBuildTokenName(member, name);

        protected IDisposable NewScope(OutputEntity entity, OutputMember member)
        {
            ILanguage language = _parameters.Language;
            string memberType = language.GetDataTypeToken(member.MemberType);
            var tokens = new Dictionary<string, object?>
            {
                ["MemberIsObsolete"] = member.IsObsolete,
                ["MemberObsoleteMessage"] = member.ObsoleteInfo?.Message ?? "",
                ["MemberObsoleteIsError"] = member.ObsoleteInfo?.IsError ?? false,
                ["MemberType"] = memberType,
                ["NativeMemberType"] = memberType,
                ["CustomMemberType"] = memberType,
                ["MemberTypeImplName"] = member.MemberType.ShortImplName,
                ["MemberTypeIntfName"] = member.MemberType.ShortIntfName,
                ["MemberTypeIntfSpace"] = member.MemberType.IntfNameSpace,
                ["MemberTypeImplSpace"] = member.MemberType.ImplNameSpace,
                ["MemberIsNullable"] = member.IsNullable,
                ["MemberSequence"] = member.Sequence,
                ["MemberName"] = member.Name,
                ["MemberJsonName"] = ToCamelCase(member.Name),
                ["MemberDefaultValue"] = language.GetDefaultValue(member.MemberType)
            };
            tokens[BuildTokenName(member, "MemberName")] = member.Name;
            tokens[BuildTokenName(member, "MemberJsonName")] = ToCamelCase(member.Name);
            tokens[BuildTokenName(member, "MemberSequence")] = member.Sequence;
            if (member.ConverterName is not null)
            {
                tokens["StructConverter"] = member.ConverterName;
            }

            // ---------- MemBlocks tokens ----------
            tokens[BuildTokenName(member, "FieldOffset")] = member.FieldOffset;
            tokens["FieldLength"] = member.FieldLength;
            //tokens["IsBigEndian"] = member.IsBigEndian;
            tokens["MemberSequenceR4"] = member.Sequence.ToString().PadLeft(4);
            tokens["FieldOffsetR4"] = member.FieldOffset.ToString().PadLeft(4);
            tokens["FieldLengthR4"] = member.FieldLength.ToString().PadLeft(4);
            tokens["MemberBELE"] = member.IsBigEndian ? "BE" : "LE";
            tokens["MemberTypeL7"] = memberType.PadRight(7);

            // ---------- MessagePack tokens ----------
            var keyOffset = entity.KeyOffset;
            if (keyOffset == 0)
            {
                keyOffset = (entity.ClassHeight - 1) * 100;
            }
            int memberKey = keyOffset + member.Sequence;
            tokens[BuildTokenName(member, "MemberKey")] = memberKey;
            return _tokenStack.NewScope(tokens);
        }

        protected IDisposable NewScope(Phase2Entity entity)
        {
            string implSpaceSuffix = entity.TFN.Impl.Space.Split('.').LastOrDefault() ?? "Generated";
            var tokens = new Dictionary<string, object?>()
            {
                ["IntfNameSpace"] = entity.TFN.Intf.Space,
                ["EntityIntfName"] = entity.TFN.Intf.Name,
                ["ImplNameSpace"] = entity.TFN.Impl.Space,
                ["EntityImplName"] = entity.TFN.Impl.Name,
                ["AbstractEntity"] = entity.TFN.Impl.Name,
                ["ConcreteEntity"] = entity.TFN.Impl.Name,
                ["EntityId"] = entity.EntityId,
                ["ClassHeight"] = entity.ClassHeight,
                ["BaseIntfNameSpace"] = entity.BaseEntity?.TFN.Intf.Space ?? SpecialName.RuntimeNamespace,
                ["BaseIntfName"] = entity.BaseEntity?.TFN.Intf.Name ?? SpecialName.RuntimeBaseIntfName,
                ["BaseImplNameSpace"] = entity.BaseEntity?.TFN.Impl.Space ?? $"{SpecialName.RuntimeNamespace}.{implSpaceSuffix}",
                ["BaseImplName"] = entity.BaseEntity?.TFN.Impl.Name ?? SpecialName.RuntimeBaseImplName,
                ["BlockLength"] = entity.BlockLength,
            };
            return _tokenStack.NewScope(tokens);
        }

        protected IDisposable NewScope(OutputEntity entity)
        {
            string implSpaceSuffix = entity.TFN.Impl.Space.Split('.').LastOrDefault() ?? "Generated";
            var tokens = new Dictionary<string, object?>()
            {
                ["IntfNameSpace"] = entity.TFN.Intf.Space,
                ["EntityIntfName"] = entity.TFN.Intf.Name,
                ["ImplNameSpace"] = entity.TFN.Impl.Space,
                ["EntityImplName"] = entity.TFN.Impl.Name,
                ["AbstractEntity"] = entity.TFN.Impl.Name,
                ["ConcreteEntity"] = entity.TFN.Impl.Name,
                ["EntityId"] = entity.EntityId,
                ["ClassHeight"] = entity.ClassHeight,
                ["BaseIntfNameSpace"] = entity.BaseEntity?.TFN.Intf.Space ?? SpecialName.RuntimeNamespace,
                ["BaseIntfName"] = entity.BaseEntity?.TFN.Intf.Name ?? SpecialName.RuntimeBaseIntfName,
                ["BaseImplNameSpace"] = entity.BaseEntity?.TFN.Impl.Space ?? $"{SpecialName.RuntimeNamespace}.{implSpaceSuffix}",
                ["BaseImplName"] = entity.BaseEntity?.TFN.Impl.Name ?? SpecialName.RuntimeBaseImplName,
                ["BlockLength"] = entity.BlockLength,
                ["BlockStructureCode"] = $"0x{entity.BlockStructureCode:X8}",
            };
            return _tokenStack.NewScope(tokens);
        }

        protected abstract void OnGenerate(OutputEntity entity);
        public string GenerateSourceText(OutputEntity entity)
        {
            using var _ = NewScope(entity);
            _builder.Clear();
            OnGenerate(entity);
            return _builder.ToString();
        }
    }
}
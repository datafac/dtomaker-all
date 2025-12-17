using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace DTOMaker.SrcGen.Core
{
    public abstract class EntityGeneratorBase
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly TokenStack _tokenStack = new TokenStack();
        private readonly ILanguage _language;

        protected EntityGeneratorBase(ILanguage language)
        {
            _language = language;
        }

        private string ReplaceTokens(string input)
        {
            // note token recursion not supported
            var tokenPrefix = _language.TokenPrefix.AsSpan();
            var tokenSuffix = _language.TokenSuffix.AsSpan();

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
                        result.Append(_language.GetValueAsCode(tokenValue));
                        remainderPos += (tokenPos + tokenPrefix.Length + tokenEnd + tokenSuffix.Length);
                        replaced = true;
                    }
                    else
                    {
                        // invalid token - emit error then original line
                        result.Clear();
                        result.AppendLine($"#error The token '{_language.TokenPrefix}{tokenName}{_language.TokenSuffix}' on the following line is invalid.");
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

        protected IDisposable NewScope(OutputMember member)
        {
            var tokens = new Dictionary<string, object?>
            {
                ["MemberIsObsolete"] = member.IsObsolete,
                ["MemberObsoleteMessage"] = member.ObsoleteMessage,
                ["MemberObsoleteIsError"] = member.ObsoleteIsError,
                ["MemberType"] = _language.GetDataTypeToken(member.MemberType),
                ["MemberTypeImplName"] = member.MemberType.ShortImplName,
                ["MemberTypeIntfName"] = member.MemberType.ShortIntfName,
                ["MemberTypeIntfSpace"] = member.MemberType.IntfNameSpace,
                ["MemberTypeImplSpace"] = member.MemberType.ImplNameSpace,
                ["MemberIsNullable"] = member.IsNullable,
                ["MemberSequence"] = member.Sequence,
                ["MemberName"] = member.Name,
                ["MemberJsonName"] = ToCamelCase(member.Name),
                ["MemberDefaultValue"] = _language.GetDefaultValue(member.MemberType)
            };
            switch (member.Kind)
            {
                case MemberKind.Native:
                    tokens["ScalarMemberSequence"] = member.Sequence;
                    tokens[(member.IsNullable ? "Nullable" : "Required") + "ScalarMemberSequence"] = member.Sequence;
                    tokens["ScalarMemberName"] = member.Name;
                    tokens[(member.IsNullable ? "Nullable" : "Required") + "ScalarMemberName"] = member.Name;
                    break;
                case MemberKind.Entity:
                    tokens[(member.IsNullable ? "Nullable" : "Required") + "EntityMemberName"] = member.Name;
                    break;
                case MemberKind.Binary:
                    tokens[(member.IsNullable ? "Nullable" : "Required") + "BinaryMemberName"] = member.Name;
                    break;
                case MemberKind.String:
                    tokens[(member.IsNullable ? "Nullable" : "Required") + "StringMemberName"] = member.Name;
                    break;
            }
            return _tokenStack.NewScope(tokens);
        }

        protected IDisposable NewScope(Phase1Entity entity)
        {
            var tokens = new Dictionary<string, object?>()
            {
                ["IntfNameSpace"] = entity.Intf.Space,
                ["EntityIntfName"] = entity.Intf.Name,
                ["ImplNameSpace"] = entity.Impl.Space,
                ["EntityImplName"] = entity.Impl.Name,
                ["AbstractEntity"] = entity.Impl.Name,
                ["ConcreteEntity"] = entity.Impl.Name,
                ["EntityId"] = entity.EntityId,
            };
            return _tokenStack.NewScope(tokens);
        }

        protected IDisposable NewScope(OutputEntity entity)
        {
            var tokens = new Dictionary<string, object?>()
            {
                ["IntfNameSpace"] = entity.Intf.Space,
                ["EntityIntfName"] = entity.Intf.Name,
                ["ImplNameSpace"] = entity.Impl.Space,
                ["EntityImplName"] = entity.Impl.Name,
                ["AbstractEntity"] = entity.Impl.Name,
                ["ConcreteEntity"] = entity.Impl.Name,
                ["EntityId"] = entity.EntityId,
                ["ClassHeight"] = entity.ClassHeight,
                ["BaseIntfNameSpace"] = entity.BaseEntity is null ? "DTOMaker.Runtime" : entity.BaseEntity.Intf.Space,
                ["BaseIntfName"] = entity.BaseEntity is null ? "IEntityBase" : entity.BaseEntity.Intf.Name,
                ["BaseImplNameSpace"] = entity.BaseEntity is null ? "System" : entity.BaseEntity.Impl.Space,
                ["BaseImplName"] = entity.BaseEntity is null ? "Object" : entity.BaseEntity.Impl.Name,
                ["DerivedEntityCount"] = entity.DerivedEntities.Count,
            };
            return _tokenStack.NewScope(tokens);
        }

        protected abstract void OnGenerate(OutputEntity scope);
        public string GenerateSourceText(OutputEntity scope)
        {
            using var _ = NewScope(scope);
            _builder.Clear();
            OnGenerate(scope);
            return _builder.ToString();
        }
    }
}
using DTOMaker.SrcGen.Core;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
namespace DTOMaker.SrcGen.MemBlocks;

public sealed partial class EntityGenerator : EntityGeneratorBase
{
    public EntityGenerator(SourceGeneratorParameters parameters) : base(parameters) { }

    protected override string OnBuildTokenName(OutputMember member, string name)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(member.IsNullable ? "Nullable" : "Required");
        if (member.Kind == MemberKind.Struct)
        {
            sb.Append(member.IsCustom ? "Custom" : "Native");
        }
        if (member.Kind == MemberKind.String || member.Kind == MemberKind.Binary)
        {
            sb.Append(member.IsEmbedded ? "FixLen" : "VarLen");
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
}
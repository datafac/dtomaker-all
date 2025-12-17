namespace DTOMaker.SrcGen.Core
{
    public class Language_CSharp : ILanguage
    {
        private static readonly Language_CSharp _instance = new Language_CSharp();
        public static Language_CSharp Instance => _instance;

        private Language_CSharp()
        {
            CommentPrefix = "//";
            CommandPrefix = "##";
            TokenPrefix = "T_";
            TokenSuffix = "_";
        }

        public string TokenPrefix { get; } = "";
        public string TokenSuffix { get; } = "";
        public string CommentPrefix { get; } = "";
        public string CommandPrefix { get; } = "";

        public string GetValueAsCode(object? value)
        {
            return value switch
            {
                null => "null",
                string s => s, // todo? identifers vs. literals
                bool b => b ? "true" : "false",
                char c => $"'{c}'",
                float f => $"{f}F",
                double d => $"{d}D",
                short s => $"{s}S",
                ushort us => $"{us}US",
                int i => $"{i}",
                uint u => $"{u}U",
                long l => $"{l}L",
                ulong ul => $"{ul}UL",
                decimal m => $"{m}M",
                System.Guid g => $"new Guid(\"{(g.ToString("D"))}\")",
                _ => $"{value}"
            };
        }

        public string GetDataTypeToken(TypeFullName typeFullName)
        {
            return typeFullName.FullName switch
            {
                KnownType.SystemString => "String",
                KnownType.MemoryOctets => "Octets",
                KnownType.PairOfInt16 => "PairOfInt16",
                KnownType.PairOfInt32 => "PairOfInt32",
                KnownType.PairOfInt64 => "PairOfInt64",
                _ => typeFullName.ShortImplName
            };
        }

        public string GetDefaultValue(TypeFullName typeFullName)
        {
            return typeFullName.FullName switch
            {
                KnownType.SystemString => "string.Empty",
                KnownType.MemoryOctets => "Octets.Empty",
                _ => "default"
            };
        }

    }
}
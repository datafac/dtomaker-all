namespace DTOMaker.CLI
{
    internal sealed class TargetLanguage_CSharp : ITargetLanguage
    {
        private static readonly TargetLanguage_CSharp _instance = new TargetLanguage_CSharp();
        public static ITargetLanguage Instance => _instance;

        public string Name => "CSharp";
        public string PrefixComment => "//";
        public string PrefixMetaCode => "##";
        public string EmitCodePrefix => "Emit(\"";
        public string EmitCodeSuffix => "\");";

        private TargetLanguage_CSharp() { }
    }
}

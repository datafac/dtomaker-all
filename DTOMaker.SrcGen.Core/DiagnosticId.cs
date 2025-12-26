namespace DTOMaker.SrcGen.Core
{
    internal static class DiagnosticId
    {
        // todo add checks for the following:
        public const string DTOM0004 = nameof(DTOM0004); // Invalid member datatype
        public const string DTOM0006 = nameof(DTOM0006); // Missing [Entity] attribute
        public const string DTOM0007 = nameof(DTOM0007); // Missing [Member] attribute
        public const string DTOM0008 = nameof(DTOM0008); // Invalid base name(s)
        public const string DTOM0009 = nameof(DTOM0009); // Duplicate entity id
        public const string DTOM0010 = nameof(DTOM0010); // Invalid entity identifier
        public const string DTOM0011 = nameof(DTOM0011); // Invalid generic entity
    }
}
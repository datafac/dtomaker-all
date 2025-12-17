namespace DTOMaker.SrcGen.Core
{
    public record class Phase1Entity
    {
        public TypeFullName TFN { get; init; } = new();
        public ParsedName Intf => TFN.Intf;
        public ParsedName Impl => TFN.Impl;
        public int EntityId { get; init; }
        public int ClassHeight { get; init; }
        public EquatableArray<OutputMember> Members { get; init; } = EquatableArray<OutputMember>.Empty;
        public TypeFullName? BaseTFN { get; init; }

        public override string ToString() => $"{TFN} [{EntityId}] ({Members.Count} members)";
    }
}

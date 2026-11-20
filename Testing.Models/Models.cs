using System;
using DTOMaker.Models;

namespace Testing.Models;

[Entity(1)]
public interface INothing : IEntityBase
{
}

[Entity(2)]
public interface ISimple : IEntityBase
{
    [Member(1)] Int64 Field1 { get; set; }
    [Member(2)] Int64? Field2 { get; set; }
    [Member(3)] String? Field3 { get; set; } // todo 32 bytes
}

[Entity(3)] public interface INumberBase : IEntityBase { }
[Entity(4)] public interface IFloatNumber : INumberBase { [Member(1)] Double Value { get; set; } }
[Entity(5)] public interface IWholeNumber : INumberBase { [Member(1)] Int64 Value { get; set; } }

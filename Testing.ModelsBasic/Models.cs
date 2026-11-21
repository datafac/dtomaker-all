using DTOMaker.Models;
using System;

namespace Testing.ModelsBasic;

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

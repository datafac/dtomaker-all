using System;
using DTOMaker.Models;

namespace Testing.Models;

[Entity(1)]
public interface ISimple : IEntityBase
{
    [Member(1)] Int64 Field1 { get; set; }
    [Member(2)] Int64? Field2 { get; set; }
}
[Entity(2)] public interface IRequired_String : IEntityBase { [Member(1)] String Field { get; set; } }

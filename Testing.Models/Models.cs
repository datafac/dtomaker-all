using System;
using DTOMaker.Models;

namespace Testing.Models;

[Entity(01)] public interface IRequired_Int64 : IEntityBase { [Member(1)] Int64 Field { get; set; } }
[Entity(02)] public interface IOptional_Int64 : IEntityBase { [Member(1)] Int64? Field { get; set; } }
[Entity(03)] public interface IRequired_String : IEntityBase { [Member(1)] String Field { get; set; } }
[Entity(04)] public interface IOptional_String : IEntityBase { [Member(1)] String? Field { get; set; } }

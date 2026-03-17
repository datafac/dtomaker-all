using System;
using DTOMaker.Models;

namespace Testing.Models;

[Entity(01)] public interface IRequired_Int64 : IEntityBase { [Member(1)] Int64 Field { get; set; } }
[Entity(02)] public interface IOptional_Int64 : IEntityBase { [Member(1)] Int64? Field { get; set; } }

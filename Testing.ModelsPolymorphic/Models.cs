using DTOMaker.Models;
using System;

namespace Testing.ModelsPolymorphic;

[Entity(3)] public interface INumberBase : IEntityBase { }
[Entity(4)] public interface IFloatNumber : INumberBase { [Member(1)] Double Value { get; set; } }
[Entity(5)] public interface IWholeNumber : INumberBase { [Member(1)] Int64 Value { get; set; } }


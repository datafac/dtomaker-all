# DTOMaker

[![Build-Deploy](https://github.com/datafac/dtomaker-all/actions/workflows/dotnet.yml/badge.svg)](https://github.com/datafac/dtomaker-all/actions/workflows/dotnet.yml)
![NuGet Version](https://img.shields.io/nuget/v/DTOMaker.Models)
![NuGet Downloads](https://img.shields.io/nuget/dt/DTOMaker.Models)
![GitHub License](https://img.shields.io/github/license/Datafac/dtomaker-all)
![GitHub Sponsors](https://img.shields.io/github/sponsors/psiman62)

*Warning: This is pre-release software under active development. Breaking changes may occur.*

Model-driven compile-time source generators for quickly creating polymorphic, freezable DTOs (Data Transport Objects) 
supporting various serialization protocols.

## Features
- Models defined via C# interfaces with attributes.
  - Source generators create implementations at compile time.
- Properties can be basic .NET types such as integers, floats, strings, Guid, etc.
  - Raw byte arrays are supported using the built-in Octets type.
  - Other common types such as DateTime, DateTimeOffset, TimeSpan are supported by built-in converters.
  - User-defined value types can be supported via user-defined converters to built-in types.
- Nullable types.
- Polymorphic types.
- Freezable types: Instances are mutable until frozen.
- Serialization protocols:
  - JSON via System.Text.Json
  - JSON via Newtonsoft.Json
  - MessagePack 2.x
  - MemBlocks
- Collections. Collections based on balanced binary trees are supported.

## Example

```C#
using DTOMaker.Models;
namespace MyModels;
[Entity(1)] public interface INode : IEntityBase
{
    [Member(1)] String Key { get; set; } 
}
[Entity(2)] public interface IStringNode : INode
{
    [Member(1)] String Value { get; set; } 
}
[Entity(3)] public interface INumberNode : INode
{
    [Member(1)] Int64 Value  { get; set; } 
}
[Entity(4)] public interface ITree : IEntityBase
{
    [Member(1)] ITree? Left  { get; set; }
    [Member(2)] ITree? Right { get; set; }
    [Member(3)] INode? Node  { get; set; }
}
```

## Workflow
```mermaid
flowchart TB
    def(Define models e.g. IMyDTO.cs)
    ref1(Reference DTOMaker.Models)
    ref2(Reference runtime e.g. DTOMaker.Runtime.JsonSystemText)
    ref3(Reference source generator e.g. DTOMaker.SrcGen.JsonSystemText)
    bld(Build/Run)
    ref1-->def
    def-->ref2
    ref2-->ref3
    ref3-->bld
```

# Development
## Coming soon
- more custom (ref and value) type converters

## Coming later
- incremental serialization
- MessagePack 3.x serialization
- Orleans serialization
- Protobuf.Net serialization
- model.json generation
- command-line alternative
- variant native type support
- variable length arrays
- logical value equality
- common pattern extensions

## How to sponsor
If you find these tools useful, please consider sponsoring my work on GitHub 
at https://github.com/sponsors/Psiman62
or buy me a coffee at https://www.buymeacoffee.com/psiman62

## License
This project is licensed under the Apache-2.0 License - see the [LICENSE](LICENSE) file for details.
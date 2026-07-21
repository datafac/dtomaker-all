using PolyType;

namespace XperPolyType.Models;

[GenerateShape]
public abstract partial record Person
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}

[GenerateShape]
public sealed partial record Student : Person
{
    public string StudentId { get; init; } = string.Empty;
}


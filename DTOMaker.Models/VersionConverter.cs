using System;
namespace DTOMaker.Models;

/// <summary>
/// When used in a <see cref="DTOMaker.Models.MemberAttribute"/>, this 
/// converter allows <see cref="System.Version"/> properties to be internally 
/// stored and serialized as <see cref="System.String"/> values.
/// </summary>
public sealed class VersionConverter : IObjectConverter<Version, String>
{
    public NativeType NativeType => NativeType.String;
    public static Version ToCustom(String native) => Version.TryParse(native, out var version) ? version : new Version();
    public static String ToNative(Version custom) => custom.ToString();
}


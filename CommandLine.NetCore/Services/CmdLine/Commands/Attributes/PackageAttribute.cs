namespace CommandLine.NetCore.Services.CmdLine.Commands.Attributes;

/// <summary>
/// this attribute put on top of a command class is used to indicates a package a command belong to
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class PackageAttribute : Attribute
{
    /// <summary>
    /// default package name
    /// </summary>
    public const string DefaultPackage = "global";

    /// <summary>
    /// package
    /// </summary>
    public string Package { get; private set; }

    /// <summary>
    /// build a new instance for one or several tags
    /// </summary>
    /// <param name="package">package</param>
    public PackageAttribute(string package)
        => Package = package;

    /// <summary>
    /// build a new instance for one or several tags
    /// </summary>
    /// <param name="package">package</param>
    public PackageAttribute(Packages package)
        => Package = package.ToString();
}

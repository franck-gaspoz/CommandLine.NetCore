namespace CommandLine.NetCore.Services.CmdLine.Commands.Attributes;

/// <summary>
/// this attribute put on top of a command class is used to associate one or several tags to the command
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class TagAttribute : Attribute
{
    /// <summary>
    /// tags
    /// </summary>
    public string[] Tags { get; private set; }

    /// <summary>
    /// build a new instance for one or several tags
    /// </summary>
    /// <param name="tags">tags</param>
    public TagAttribute(params string[] tags)
        => Tags = tags;

    /// <summary>
    /// build a new instance for one or several tags
    /// </summary>
    /// <param name="tags">tags</param>
    public TagAttribute(params object[] tags)
        => Tags = tags.Select(x => x.ToString()!)
            .ToArray();
}

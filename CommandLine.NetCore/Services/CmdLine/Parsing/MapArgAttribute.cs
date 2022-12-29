namespace CommandLine.NetCore.Services.CmdLine.Parsing;

/// <summary>
/// indicates that a command operation method parameter maps an instance of arg
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class MapArgAttribute : Attribute
{
    /// <summary>
    /// the arg index that is mapped to the command operation method parameter
    /// </summary>
    public int MappedArg { get; private set; }

    /// <summary>
    /// indicates that a command operation method parameter maps an instance of arg
    /// </summary>
    /// <param name="mappedArg">the arg index that is mapped to the command operation method parameter</param>
    public MapArgAttribute(int mappedArg) => MappedArg = mappedArg;
}

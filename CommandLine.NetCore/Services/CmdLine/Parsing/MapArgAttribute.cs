namespace CommandLine.NetCore.Services.CmdLine.Parsing;

/// <summary>
/// indicates that a command operation method parameter maps an instance of arg
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class MapArgAttribute : Attribute
{
    /// <summary>
    /// the arg index that is mapped to the command operation method parameter
    /// </summary>
    public int ArgIndex { get; private set; }

    /// <summary>
    /// indicates that a command operation method parameter maps an instance of arg
    /// </summary>
    /// <param name="mappedArgIndex">the arg index that is mapped to the command operation method parameter</param>
    public MapArgAttribute(int mappedArgIndex) => ArgIndex = mappedArgIndex;
}

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// a parameter in a set of command line arguments
/// </summary>
public interface IParam : IArg
{
    /// <summary>
    /// string representation of the value
    /// </summary>
    string? StringValue { get; }

    /// <summary>
    /// try to assign the value from the string representation of the value
    /// </summary>
    /// <param name="value">string representation of the value</param>
    /// <exception cref="ArgumentException">convert from string to value error</exception>
    public void SetValue(string value);
}
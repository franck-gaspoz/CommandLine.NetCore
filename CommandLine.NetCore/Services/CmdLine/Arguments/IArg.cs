namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// an argument of a command line argument
/// </summary>
public interface IArg
{
    /// <summary>
    /// returns a syntax representation of this param
    /// </summary>
    /// <returns></returns>
    string ToSyntax();

    /// <summary>
    /// type of the argument value
    /// </summary>
    Type ValueType { get; }

    /// <summary>
    /// returns the value of the argument as it is stored after parsing
    /// </summary>
    /// <returns>the value of the argument as it is stored after parsing</returns>
    object? GetValue();
}
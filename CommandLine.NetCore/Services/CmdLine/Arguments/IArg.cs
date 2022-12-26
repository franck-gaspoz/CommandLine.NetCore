namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// an argument of a command line argument
/// </summary>
public interface IArg
{
    /// <summary>
    /// returns a grammar representation of this param
    /// </summary>
    /// <returns></returns>
    public string ToGrammar();
}
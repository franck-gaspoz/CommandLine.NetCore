namespace CommandLine.NetCore.Services.CmdLine.Arguments;

public interface IParam
{
    /// <summary>
    /// returns a grammar representation of this param
    /// </summary>
    /// <returns></returns>
    public string ToGrammar();
}

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

public interface IOpt
{
    string Name { get; }
    string Prefix { get; }
    string PrefixedName { get; }
    int ExpectedValuesCount { get; }

    void AddValue(string value);
    bool GetDescription(out KeyValuePair<string, string> description);

    /// <summary>
    /// returns a grammar representation of this param
    /// </summary>
    /// <returns></returns>
    public string ToGrammar();
}

using CommandLine.NetCore.Services.Text;

using static CommandLine.NetCore.Services.CmdLine.Globals;
namespace CommandLine.NetCore.Services.CmdLine.Arguments;

public sealed class Parser
{
    private readonly Texts _texts;

    public Parser(Texts texts) => _texts = texts;

    public static string ClassNameToOptName(string name)
        => name[0..^9]
            .ToLower();

    public static string GetPrefixFromClassName(string name)
    {
        var argName = ClassNameToOptName(name);
        return argName.Length == 1
            ? ShortArgNamePrefix
            : LongArgNamePrefix;
    }

    public static string GetPrefixFromArgName(string name)
        => name.Length == 1
            ? ShortArgNamePrefix
            : LongArgNamePrefix;

    /// <summary>
    /// return true if the text is the syntax of an opt name (starst with - ot --)
    /// </summary>
    /// <param name="text">text to be checked</param>
    /// <returns>true if syntax max an opt name</returns>
    public static bool IsOpt(string text)
        => text.StartsWith(ShortArgNamePrefix)
            || text.StartsWith(LongArgNamePrefix);

    /// <summary>
    /// extract values from an arguments lists. try to get the expectd values count
    /// </summary>
    /// <param name="opt">parsed option</param>
    /// <param name="args">arg list. the list is consumed (elements are removed)</param>
    /// <param name="index">begin index</param>
    /// <param name="position">actual begin index in arguments list</param>
    /// <exception cref="ArgumentException">missing argument value</exception>
    public void ParseOptValues(
        IOpt opt,
        List<string> args,
        int index,
        int position)
    {
        var expectedCount = opt.ExpectedValuesCount;
        args.RemoveAt(index);
        while (expectedCount > 0)
        {
            position++;
            if (!args.Any() || !Parser.IsOpt(args[index]))
            {
                throw new ArgumentException(
                    _texts._("MissingArgumentValue", position, opt.Name));
            }

            opt.AddValue(args[index]);
            args.RemoveAt(index);
            expectedCount--;
        }
    }
}

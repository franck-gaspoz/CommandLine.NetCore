
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

using static CommandLine.NetCore.Services.CmdLine.Globals;

namespace CommandLine.NetCore.Service.CmdLine.Arguments;

/// <summary>
/// argument abstraction
/// </summary>
public class Arg
{
    /// <summary>
    /// app config
    /// </summary>
    protected readonly IConfiguration Config;

    /// <summary>
    /// texts
    /// </summary>
    protected readonly Texts Texts;

    /// <summary>
    /// argument name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// values count (expected)
    /// </summary>
    public int ValuesCount { get; private set; }

    /// <summary>
    /// unparsed values
    /// </summary>
    protected readonly List<string> Values = new();

    /// <summary>
    /// build a new argument
    /// <para>corresponds to the syntax:</para>
    /// <para>-argName | --argName [value1 [.. valuen]]</para>
    /// </summary>
    /// <param name="name">argument name</param>
    /// <param name="config">app config</param>
    /// <param name="texts">texts</param>
    /// <param name="valuesCount">number of expected values</param>
    protected Arg(
        string name,
        IConfiguration config,
        Texts texts,
        int valuesCount = 0)
    {
        Name = name;
        Config = config;
        Texts = texts;
        ValuesCount = valuesCount;
    }

    /// <summary>
    /// get the argument description from help settings
    /// </summary>
    /// <param name="description">description or error message if not found</param>
    /// <returns>true if description is found, otherwise false</returns>
    public bool GetDescription(out KeyValuePair<string, string> description)
    {
        var desc = Config.GetSection("GlobalArgs:" + Name);

        if (!desc.Exists() || !desc.GetChildren().Any())
        {
            description = new KeyValuePair<string, string>(
                Texts._("GlobalArgHelpNotFound", Name),
                string.Empty);
            return false;
        }

        var kvp = desc.GetChildren().First();

        description = new KeyValuePair<string, string>(
            kvp.Key,
            kvp.Value ?? string.Empty
            );

        return true;
    }

    /// <summary>
    /// extract values form an arguments lists. try to get the expectd values count
    /// </summary>
    /// <param name="args">arg list</param>
    /// <param name="index">begin index</param>
    /// <param name="position">actual begin index in arguments list</param>
    /// <exception cref="ArgumentException">missing argument value</exception>
    public void ParseValues(List<string> args, int index, int position)
    {
        var expectedCount = ValuesCount;
        args.RemoveAt(index);
        while (expectedCount > 0)
        {
            position++;
            if (!args.Any() || !IsArg(args[index]))
                throw new ArgumentException(Texts._("MissingArgumentValue", position, Name));

            Values.Add(args[index]);
            args.RemoveAt(index);
            expectedCount--;
        }
        Initialize();
    }

    protected virtual void Initialize() { }

    #region translaters, helpers

    public string Prefix => GetPrefixFromArgName(Name);

    public static bool IsArg(string text)
        => text.StartsWith(ShortArgNamePrefix)
            || text.StartsWith(LongArgNamePrefix);

    public static string ClassNameToArgName(string name)
        => name[0..^9]
            .ToLower();

    public static string GetPrefixFromClassName(string name)
    {
        var argName = ClassNameToArgName(name);
        return argName.Length == 1
            ? ShortArgNamePrefix
            : LongArgNamePrefix;
    }

    public static string GetPrefixFromArgName(string name)
        => name.Length == 1
            ? ShortArgNamePrefix
            : LongArgNamePrefix;

    #endregion
}

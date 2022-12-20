using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

using static CommandLine.NetCore.Services.CmdLine.Globals;

namespace CommandLine.NetCore.Service.CmdLine.Arguments.GlobalArgs;

internal abstract class Arg
{
    protected readonly IConfiguration _config;
    protected readonly Texts _texts;

    public string Name { get; private set; }

    public int ParametersCount { get; private set; }

    protected readonly List<string> _parameters = new();
    public IReadOnlyList<string> Parameters => _parameters;

    public Arg(
        string name,
        IConfiguration config,
        Texts texts,
        int parametersCount = 0)
    {
        Name = name;
        _config = config;
        _texts = texts;
        ParametersCount = parametersCount;
    }

    public KeyValuePair<string, string> Description()
    {
        var desc = _config.GetSection("GlobalArgs:" + Name);

        if (!desc.Exists() || !desc.GetChildren().Any())
        {
            return new KeyValuePair<string, string>(
                _texts._("GlobalArgHelpNotFound", Name),
                string.Empty);
        }

        var kvp = desc.GetChildren().First();

        return new KeyValuePair<string, string>(
            kvp.Key,
            kvp.Value ?? string.Empty
            );
    }

    public string Prefix => GetPrefixFromArgName(Name);

    public void ParseParameters(List<string> args, int index, int position)
    {
        var expectedCount = ParametersCount;
        args.RemoveAt(index);
        while (expectedCount > 0)
        {
            position++;
            if (!args.Any())
                throw new ArgumentException(_texts._("MissingArgumentValue", position, Name));
            _parameters.Add(args[index]);
            args.RemoveAt(index);
            expectedCount--;
        }
        Initialize();
    }

    protected virtual void Initialize() { }

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
}

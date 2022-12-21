
using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Commands;

/// <summary>
/// abstract command
/// </summary>
public abstract class Command
{
    /// <summary>
    /// console service
    /// </summary>
    protected readonly IAnsiVtConsole Console;

    /// <summary>
    /// texts service
    /// </summary>
    protected readonly Texts Texts;

    /// <summary>
    /// app config
    /// </summary>
    protected readonly IConfiguration Config;

    /// <summary>
    ///  min arg count
    /// </summary>
    protected readonly int? MinArgCount;

    /// <summary>
    /// max arg count
    /// </summary>
    protected readonly int? MaxArgCount;

    /// <summary>
    /// name of the command
    /// </summary>
    public string Name => ClassNameToCommandName();

    private readonly ArgBuilder _argBuilder;

    /// <summary>
    /// construit une instance de commande
    /// </summary>
    /// <param name="config">app config</param>
    /// <param name="console">console service</param>
    /// <param name="texts">texts service</param>
    /// <param name="argBuilder">args builder</param>
    /// <param name="minArgCount">command minimum args count</param>
    /// <param name="maxArgCount">command maximum args count</param>
    public Command(
        IConfiguration config,
        IAnsiVtConsole console,
        Texts texts,
        ArgBuilder argBuilder,
        int minArgCount = 0,
        int maxArgCount = 0)
    {
        _argBuilder = argBuilder;
        Config = config;
        Texts = texts;
        Console = console;
        MinArgCount = minArgCount;
        MaxArgCount = maxArgCount;
    }

    /// <summary>
    /// exec command
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>return code</returns>
    public int Run(ArgSet args) => Execute(args);

    /// <summary>
    /// command run body to be implemented by subclasses
    /// </summary>
    /// <param name="args">args</param>
    /// <returns>return code</returns>
    /// <exception cref="NotImplementedException">not implemented</exception>
    protected abstract int Execute(ArgSet args);

    /// <summary>
    /// short description of the command
    /// </summary>
    /// <param name="text">short description or error text if not found in help settings</param>
    /// <returns>text of the description</returns>
    public bool GetDescription(out string text)
    {
        var desc = Config.GetValue<string>($"Commands:{ClassNameToCommandName()}:Description");
        if (desc is not null)
        {
            text = desc;
            return true;
        }
        text = Console.Colors.Error + Texts._("CommandShortHelpNotFound", ClassNameToCommandName());
        return false;
    }

    /// <summary>
    /// long descriptions of the command
    /// </summary>
    /// <param name="texts">command syntaxes descriptions or error text if not found in help settings</param>
    /// <returns>text of the descriptions of each command syntax. key is the syntax, value is the description</returns>
    public bool GetLongDescriptions(out List<KeyValuePair<string, string>> texts)
    {
        var descs = Config.GetSection($"Commands:{ClassNameToCommandName()}:Syntax");

        if (!descs.Exists() || !descs.GetChildren().Any())
        {
            texts = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string,string>(
                    Console.Colors.Error +
                    Texts._("CommandLongHelpNotFound", ClassNameToCommandName()),
                    string.Empty)
            };
            return false;
        }

        texts = new List<KeyValuePair<string, string>>();
        foreach (var desc in descs.GetChildren())
        {
            texts.Add(
                new KeyValuePair<string, string>(
                    desc.Key, desc.Value ?? string.Empty));
        }

        return true;
    }

    #region translaters, helpers

    /// <summary>
    /// returns the command name from this class type
    /// </summary>
    /// <returns>command name</returns>
    public string ClassNameToCommandName()
        => ClassNameToCommandName(GetType().Name);

    /// <summary>
    /// transforms a class type name to a commande name
    /// </summary>
    /// <param name="className">command type name</param>
    /// <returns>command name</returns>
    public static string ClassNameToCommandName(string className)
        => className[0..^7].ToLower();

    /// <summary>
    /// transforms a commande name to a class type name
    /// </summary>
    /// <param name="name">command name</param>
    /// <returns>name of command type</returns>
    public static string CommandNameToClassType(string name)
        => name.ToFirstUpper() + typeof(Command).Name;

    #endregion

    #region args helpers

    /// <summary>
    /// check args count doesn't exceed max arg count allowed
    /// </summary>
    /// <param name="args">args</param>
    /// <exception cref="ArgumentException">too many arguments</exception>
    protected void CheckMaxArgs(string[] args)
    {
        if (MaxArgCount is null)
            return;
        if (args.Length > MaxArgCount)
            throw new ArgumentException(Texts._("TooManyArguments", MaxArgCount));
    }

    /// <summary>
    /// check args count is not lower than min allowed arg count
    /// </summary>
    /// <param name="args">args</param>
    /// <exception cref="ArgumentException">not enough arguments</exception>
    protected void CheckMinArgs(string[] args)
    {
        if (MinArgCount is null)
            return;
        if (args.Length > MinArgCount)
            throw new ArgumentException(Texts._("NotEnoughArguments", MinArgCount));
    }

    /// <summary>
    /// check args count doesn't exceed max arg count allowed and args count is not lower than min allowed arg count
    /// </summary>
    /// <param name="args">args</param>
    /// <exception cref="ArgumentException">too many arguments</exception>
    /// <exception cref="ArgumentException">not enough arguments</exception>
    protected void CheckMinMaxArgs(string[] args)
    {
        CheckMaxArgs(args);
        CheckMinArgs(args);
    }

    protected void Arg(string name, int valuesCount = 0)
        => _argBuilder

    #endregion
}


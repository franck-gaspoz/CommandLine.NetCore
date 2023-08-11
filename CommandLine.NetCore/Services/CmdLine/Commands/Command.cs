using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Extensions;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// abstract command
/// </summary>
public abstract class Command : CommandBuilder
{
    #region properties

    /// <summary>
    /// app config
    /// </summary>
    protected readonly IConfiguration Config;

    /// <summary>
    /// name of the command
    /// </summary>
    public string Name => ClassNameToCommandName();

    #endregion

    /// <summary>
    /// builds a new command
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    public Command(Dependencies dependencies)
        : base(dependencies) => Config = dependencies.GlobalSettings.Configuration;

    /// <summary>
    /// run the command with the specified arguments
    /// </summary>
    /// <param name="args">args</param>
    /// <returns>return code</returns>
    /// <exception cref="NotImplementedException">not implemented</exception>
    public CommandResult Run(ArgSet args)
    {
        var commandResult = Execute(args);

        if (commandResult.ParseErrors.Any())
        {
            Console.Logger.LogError(
                string.Join(
                    Environment.NewLine,
                    commandResult.ParseErrors)
                + "(br)");
        }

        return commandResult;
    }

    /// <summary>
    /// command run body to be implemented by subclasses
    /// </summary>
    /// <param name="args">args</param>
    /// <returns>return code</returns>
    /// <exception cref="NotImplementedException">not implemented</exception>
    protected abstract CommandResult Execute(ArgSet args);

    #region command help

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
        text = Console.Colors.Error + Texts._(
            "CommandShortHelpNotFound",
            ClassNameToCommandName());
        return false;
    }

    /// <summary>
    /// get descriptions for the command options
    /// </summary>
    /// <param name="texts">founded descriptions or empty list</param>
    /// <returns>true if some options exists</returns>
    public bool GetOptionsDescriptions(out List<KeyValuePair<string, string>> texts)
    {
        var sectionName = $"Commands:{ClassNameToCommandName()}:Options";
        var optDescs = Config.GetSection(sectionName);

        if (!optDescs.Exists() || !optDescs.GetChildren().Any())
        {
            texts = new List<KeyValuePair<string, string>>();
            return false;
        }

        texts = new List<KeyValuePair<string, string>>();
        foreach (var optDesc in optDescs.GetChildren())
        {
            texts.Add(
                new KeyValuePair<string, string>(
                    optDesc.Key,
                    optDesc.Value ?? string.Empty
                ));
        }
        return true;
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
                    Texts._("CommandLongHelpNotFound",
                        ClassNameToCommandName()),
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

    #endregion

    #region command run

    /// <summary>
    /// run a command from command line arguments
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>operation result</returns>
    public override OperationResult RunCommand(params string[] args)
    {
        var commandLineInterfaceBuilder = GlobalSettings
            .CommandLineInterfaceBuilder!;

        var serviceProvider =
            commandLineInterfaceBuilder
            .AppHost!
            .Services!;

        serviceProvider
            .ConfigureCommandLineArgs(args)
            .ConfigureGlobalSettings()
            .ConfigureOutput();

        return new(
            commandLineInterfaceBuilder.Run()
            );
    }

    /// <summary>
    /// run a command from command line arguments in a new host
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>operation result</returns>
    public OperationResult RunCommandInSeparateHost(params string[] args) =>
        new(
            new CommandLineInterfaceBuilder()
                .UseAssemblySet(GlobalSettings.AssemblySet)
                .UseConfigureDelegate(GlobalSettings
                    .AppHostConfiguration
                    .ConfigureDelegate)
                .UseBuildDelegate(GlobalSettings
                    .AppHostConfiguration
                    .BuildDelegate)
                .Build(args)
                .Run());

    #endregion
}


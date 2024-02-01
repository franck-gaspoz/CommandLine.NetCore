using System.Diagnostics;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.AppHost;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Extensions;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Running;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// abstract command
/// </summary>
[DebuggerDisplay("{Name}")]
public abstract partial class Command
{
    #region properties

    /// <summary>
    /// console service
    /// </summary>
    protected readonly IAnsiVtConsole Console;

    /// <summary>
    /// texts service
    /// </summary>
    protected readonly Texts Texts;

    /// <summary>
    /// parser
    /// </summary>
    protected readonly Parser Parser;

    /// <summary>
    /// setted global options
    /// </summary>
    protected readonly GlobalSettings GlobalSettings;

    /// <summary>
    /// command builder
    /// </summary>
    readonly CommandBuilder _builder;

    /// <summary>
    /// app config
    /// </summary>
    protected readonly Configuration Config;

    /// <summary>
    /// name of the command
    /// </summary>
    public virtual string Name => ClassNameToCommandName();

    /// <summary>
    /// command specification
    /// </summary>
    protected SyntaxMatcherDispatcher SyntaxMatcherDispatcher { get; set; }

    /// <summary>
    /// logger
    /// </summary>
    protected CoreLogger Logger { get; private set; }

    #endregion

    /// <summary>
    /// builds a new command
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    public Command(Dependencies dependencies)
    {
        Console = dependencies.Console;
        Texts = dependencies.Texts;
        Parser = dependencies.Parser;
        GlobalSettings = dependencies.GlobalSettings;
        Logger = dependencies.Logger;

        _builder = new(
            dependencies,
            ClassNameToCommandName(),
            RunCommand);

        Config = dependencies
                .GlobalSettings
                .Configuration;

        SyntaxMatcherDispatcher = Declare();
    }

    /// <summary>
    /// run the command with the specified arguments
    /// </summary>
    /// <param name="args">args</param>
    /// <returns>return code</returns>
    /// <exception cref="NotImplementedException">not implemented</exception>
    public CommandResult Run(ArgSet args)
    {
        var commandResult = SyntaxMatcherDispatcher.With(args);

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
    /// declares the command syntax and implementation mappings
    /// </summary>
    /// <returns></returns>
    protected abstract SyntaxMatcherDispatcher Declare();

    #region translaters, helpers

    /// <summary>
    /// returns the command name from this class type
    /// </summary>
    /// <returns>command name</returns>
    public virtual string ClassNameToCommandName()
        => ClassNameToCommandName(GetType().Name);

    /// <summary>
    /// returns a command name from a command class type
    /// </summary>
    /// <param name="t">command type</param>
    /// <returns>command name</returns>
    public static string ClassNameToCommandName(Type t)
        => ClassNameToCommandName(t.Name);

    /// <summary>
    /// transforms a class type name to a commande name
    /// </summary>
    /// <param name="className">command type name</param>
    /// <returns>command name</returns>
    public static string ClassNameToCommandName(string className)
        => className.ToKebabCase()!;

    #endregion

    #region command run

    /// <summary>
    /// run a command from command line arguments
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>operation result</returns>
    public CommandResult RunCommand(params string[] args)
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

        return commandLineInterfaceBuilder.Run();
    }

    /// <summary>
    /// run a command from command line arguments in a new host
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>operation result</returns>
    public CommandResult RunCommandInSeparateHost(params string[] args)
        => new CommandLineInterfaceBuilder()
                .UseAssemblySet(GlobalSettings.AssemblySet)
                .UseConfigureDelegate(GlobalSettings
                    .AppHostConfiguration
                    .ConfigureDelegate)
                .UseBuildDelegate(GlobalSettings
                    .AppHostConfiguration
                    .BuildDelegate)
                .Build(args)
                .Run();

    #endregion
}


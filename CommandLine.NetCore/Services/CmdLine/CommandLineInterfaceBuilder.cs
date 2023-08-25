using System.Reflection;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Commands.CmdLine;
using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.GlobalOpts;
using CommandLine.NetCore.Initializer;
using CommandLine.NetCore.Services.AppHost;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Running.Exceptions;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Error;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static CommandLine.NetCore.Services.CmdLine.Settings.Globals;

using cons = AnsiVtConsole.NetCore;

namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// command line interface
/// </summary>
public sealed class CommandLineInterfaceBuilder
{
    #region properties

    const string ErrorTypeNameMessageTexteSeparator = ": ";

    readonly AssemblySet _assemblySet;

    Action<IConfigurationBuilder>? _configureDelegate;

    Action<IHostBuilder>? _buildDelegate;

    AppHostBuilder? _appHostBuilder;

    bool _isRunning;

    Type? _forCommandType;

    string? _forCommandName;

    bool _isGlobalHelpEnabled = true;
    static bool _isDumpStackTrace = false;

    /// <summary>
    /// app host
    /// </summary>
    internal IHost? AppHost => _appHostBuilder?.AppHost;

    readonly Dictionary<string, DynamicCommandSpecification> _dynamicCommands = new();

    readonly List<ErrorDescriptor> _initializationErrors = new();

    #endregion

    /// <summary>
    /// creates a new instance builder
    /// </summary>
    public CommandLineInterfaceBuilder()
    {
        _assemblySet = new AssemblySet(Assembly.GetCallingAssembly());
        UseAssembly(Assembly.GetExecutingAssembly());
    }

    #region configure methods

    bool CheckForCommandIsAccepted()
    {
        if (_forCommandType is not null)
            _initializationErrors.Add(
                this.MulitpleFor(_forCommandType.Name));
        if (_forCommandName is not null)
            _initializationErrors.Add(
                this.MulitpleFor(_forCommandName));
        return _forCommandName is null
            && _forCommandType is null;
    }

    /// <summary>
    /// configure the command line to work for an unique command
    /// <para>only the command of the specified type is available</para>
    /// <para>the name of the command is not required in the command line, because it is implicit</para>
    /// </summary>
    /// <typeparam name="CommandType">type of the command</typeparam>
    /// <returns>this object</returns>
    public CommandLineInterfaceBuilder ForCommand<CommandType>()
        where CommandType : Command
    {
        if (CheckForCommandIsAccepted())
            _forCommandType = typeof(CommandType);
        return this;
    }

    /// <summary>
    /// configure the command line to work for an unique command
    /// </summary>
    /// <param name="commandName">command name</param>
    /// <returns>this object</returns>
    public CommandLineInterfaceBuilder ForCommand(string commandName)
    {
        if (CheckForCommandIsAccepted())
            _forCommandName = commandName;
        return this;
    }

    /// <summary>
    /// disable global help - often associated with ForCommand
    /// </summary>
    /// <returns>this object</returns>
    public CommandLineInterfaceBuilder DisableGlobalHelp()
    {
        _isGlobalHelpEnabled = false;
        return this;
    }

    /// <summary>
    /// add an assembly where to look up for commands and arguments
    /// <para>this can called multiple times</para>
    /// </summary>
    /// <param name="assembly">assembly</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder UseAssembly(Assembly assembly)
    {
        if (!_assemblySet.Assemblies.Contains(assembly))
            _assemblySet.Assemblies.Add(assembly);
        return this;
    }

    /// <summary>
    /// use assemblies from an assembly set
    /// </summary>
    /// <param name="assemblySet">assembly set</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder UseAssemblySet(AssemblySet assemblySet)
    {
        foreach (var assembly in assemblySet.Assemblies)
            UseAssembly(assembly);
        return this;
    }

    /// <summary>
    /// set up a configure delegate
    /// </summary>
    /// <param name="configureDelegate">configure delegate</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder UseConfigureDelegate(Action<IConfigurationBuilder>? configureDelegate)
    {
        if (configureDelegate is null)
            return this;
        _configureDelegate = configureDelegate;
        return this;
    }

    /// <summary>
    /// set up a build delegate
    /// </summary>
    /// <param name="buildDelegate">build delegate</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder UseBuildDelegate(Action<IHostBuilder>? buildDelegate)
    {
        if (buildDelegate is null)
            return this;
        _buildDelegate = buildDelegate;
        return this;
    }

    /// <summary>
    /// adds a command specification and implementation (classless)
    /// </summary>
    /// <param name="name">name of the command (as in the command line)</param>
    /// <param name="specificationDelegate">command specification delegate</param>
    /// <param name="helpBuilder">helper builder</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder AddCommand(
        string name,
        DynamicCommandSpecificationDelegate specificationDelegate,
        HelpBuilder? helpBuilder = null
        )
    {
        if (_dynamicCommands.ContainsKey(name))
            _initializationErrors.Add(
                this.CommandAlreadyExists(
                    name,
                    new
                    {
                        Execute = specificationDelegate
                    }));
        else
            _dynamicCommands.Add(
                name,
                new DynamicCommandSpecification(
                    name,
                    specificationDelegate,
                    helpBuilder));

        return this;
    }

    #endregion

    #region build & run

    /// <summary>
    /// run the command line
    /// <para>run the command according to the command line arguments</para>
    /// <para>handle errors</para>
    /// <para>output to console command results and any error</para>
    /// <para>run the host</para>
    /// </summary>
    /// <returns>return code of the command</returns>
    /// <exception cref="ArgumentException">argument error</exception>
    public int Run()
    {
        try
        {
            var host = _appHostBuilder!.AppHost;
            if (!_isRunning)
            {
                host.RunAsync();
                _isRunning = true;
            }

            var texts = host.Services.GetRequiredService<Texts>();
            var console = host.Services.GetRequiredService<IAnsiVtConsole>();
            var commandSet = host.Services.GetRequiredService<CommandsSet>();
            var args = host.Services.GetRequiredService<CommandLineArgs>();
            var parser = host.Services.GetRequiredService<Parser>();
            var globalSettings = host.Services.GetRequiredService<GlobalSettings>();
            globalSettings.SetCommandLineBuilder(this);
            var lineBreak = false;

            if (!_isGlobalHelpEnabled)
                globalSettings
                    .SettedGlobalOptsSet
                    .Add((
                        new DisableGlobalHelp(
                            globalSettings.Configuration,
                            texts,
                            new ValueConverter(texts)),
                        new List<string>()));

            _isGlobalHelpEnabled = !globalSettings.IsGlobalOptionSet<DisableGlobalHelp>();

            try
            {
                var helpCommand = host.Services.GetRequiredService<Help>();
                var anyArg = args.Args.Any();
                var isNotHelpCommand = anyArg && args.Args.First() != helpCommand.Name;

                if (!args.Any() && (_forCommandType is null || !isNotHelpCommand))
                    throw new ArgumentException(texts._("MissingArguments"));

                console.Out.WriteLine();
                lineBreak = true;

                var filteredArgs = (_forCommandType is null && anyArg || !isNotHelpCommand) ?
                    args.Args.ToArray()[1..]
                    : args.Args.ToArray();

                Command command;

                if (args.Count == 1
                    && args.Args.First() == helpCommand.Name
                    && !_isGlobalHelpEnabled)
                    throw new ArgumentException(
                        texts._("UnknownCommand", helpCommand.Name));

                var secondArg = args.Args.Count > 1 ? args.Args.ElementAt(1) : null;
                var forCommand = _forCommandType is not null ?
                    (Command)host.Services.GetRequiredService(_forCommandType)
                    : null;

                if (_forCommandType is not null
                    && (isNotHelpCommand || !_isGlobalHelpEnabled)
                        && (!(!isNotHelpCommand
                            && (secondArg is not null
                                && secondArg == forCommand!.Name))))
                    command = forCommand!;
                else
                    command = commandSet.GetCommand(args.Args.ToArray()[0]);

                var commandResult = command.Run(
                    new ArgSet(filteredArgs));

                console.Out.WriteLine();

                return commandResult.ExitCode;
            }
            catch (MissingOrNotFoundCommandOperationException missingOrNotFoundCommandOperationException)
            {
                return ExitWithError(
                    texts._("MissingOrNotFoundCommandOperation",
                        missingOrNotFoundCommandOperationException.Details),
                    console,
                    lineBreak);
            }
            catch (InvalidCommandOperationException invalidCommandOperation)
            {
                return ExitWithError(
                    texts._(
                        "InvalidCommandOperation",
                        invalidCommandOperation.Details),
                    console,
                    lineBreak);
            }
            catch (InvalidCommandOperationParameterCastException invalidCommandOperationParameterCastException)
            {
                return ExitWithError(
                    texts._(
                        "InvalidCommandOperationParameterCast",
                        invalidCommandOperationParameterCastException.Index,
                        invalidCommandOperationParameterCastException.SourceArgument.GetType().FriendlyName(),
                        invalidCommandOperationParameterCastException.TargetParameter.ParameterType.FriendlyName(),
                        invalidCommandOperationParameterCastException.Details),
                    console,
                    lineBreak);
            }
            catch (TargetInvocationException invokeCommandOperationExecutionException)
            {
                return ExitWithError(
                    invokeCommandOperationExecutionException.InnerException!,
                    console,
                    lineBreak);
            }
            catch (Exception commandExecutionException)
            {
                return ExitWithError(
                    commandExecutionException,
                    console,
                    lineBreak);
            }
        }
        catch (Exception hostBuilderException)
        {
            return ExitWithError(
                hostBuilderException,
                new cons.AnsiVtConsole(),
                false);
        }
    }

#if no
    /// <summary>
    /// get a command from service provider, by type (class command) or name (dynamic command)
    /// </summary>
    /// <param name="texts">texts</param>
    /// <param name="serviceProvider">service provider</param>
    /// <param name="commandType">type of command (if any)</param>
    /// <param name="commandName">name of command (if any)</param>
    /// <returns>command</returns>
    /// <exception cref="ArgumentException">unknwown command</exception>
    Command GetCommand(
        Texts texts,
        IServiceProvider serviceProvider,
        Type? commandType = null,
        string? commandName = null,
        CommandsSet commandsSet,
        DynamicCommandsSet dynamicCommandsSet)
    {
        if (commandType is null && commandName is null)
            throw new ArgumentException(
                texts._(
                    "UnknownCommand","null");

        if (commandType is not null) serviceProvider.GetReqService(commandType);

    }
#endif

    /// <summary>
    /// build the command line interface async host initialized for the command line arguments
    /// <para>initalize the injectables dependencies</para>
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder Build(string[] args)
    {
        try
        {
            _appHostBuilder = new AppHostBuilder(
                args.ToList(),
                new AppHostConfiguration(
                    _assemblySet,
                    _isGlobalHelpEnabled,
                    _forCommandType,
                    _forCommandName,
                    _configureDelegate,
                    _buildDelegate,
                    _dynamicCommands,
                    _initializationErrors));
        }
        catch (Exception hostBuilderException)
        {
            _isDumpStackTrace = true;
            Environment.Exit(
                ExitWithError(
                    hostBuilderException,
                    new cons.AnsiVtConsole(),
                    false));
        }
        return this;
    }

    #endregion

    #region errors fallbacks

    static int ExitWithError(
        Exception ex,
        IAnsiVtConsole? console = null,
        bool lineBreak = true)
            => ExitWithError(
                (ex is INotExplicitMessageException ?
                    ex.GetType().Name
                    + ErrorTypeNameMessageTexteSeparator
                    + Environment.NewLine
                : string.Empty)
                + ex.Message
                + (_isDumpStackTrace ?
                    Environment.NewLine + ex.StackTrace
                    : string.Empty),
                console,
                lineBreak);

    static int ExitWithError(
        string error,
        IAnsiVtConsole? console = null,
        bool lineBreak = true)
    {
        if (!lineBreak)
            LogError(console);
        LogError(error, console);
        LogError(console);
        return ExitFail;
    }

    static void LogError(IAnsiVtConsole? console = null)
    {
        if (console is null)
            Console.Error.WriteLine(string.Empty);
        else
            console!.Logger.LogError(string.Empty);
    }

    static void LogError(string error = "", IAnsiVtConsole? console = null)
    {
        if (console is null)
            Console.Error.WriteLine(error);
        else
            console!.Logger.LogError(error);
    }

    #endregion
}

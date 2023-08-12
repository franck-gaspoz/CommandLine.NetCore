using CommandLine.NetCore.Services.AppHost;
using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// set of dynamic commands operations &amp; store
/// </summary>
sealed class DynamicCommandsSet
{
    readonly Texts _texts;
    readonly IServiceProvider _serviceProvider;
    readonly Dependencies _dependencies;

    /// <summary>
    /// creates a new dynamic commands set
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    /// <param name="texts">text</param>
    /// <param name="serviceProvider"></param>
    /// <param name="appHostConfiguration"></param>
    public DynamicCommandsSet(
        Dependencies dependencies,
        Texts texts,
        IServiceProvider serviceProvider,
        AppHostConfiguration appHostConfiguration)
    {
        _dependencies = dependencies;
        _texts = texts;
        _serviceProvider = serviceProvider;

        foreach (var commandSpec in GetCommands(appHostConfiguration))
            Add(commandSpec);
    }

    /// <summary>
    /// return selectables types from host configuration
    /// <para>if unique command, returns the unique command</para>
    /// <para>'Help' command is always added if found</para>
    /// </summary>
    /// <param name="appHostConfiguration">app host configuration</param>
    /// <returns>list of selected dynamic command types</returns>
    static IEnumerable<DynamicCommandExecuteMethod> GetCommands(
        AppHostConfiguration appHostConfiguration)
    {
        var selectedCommands = new List<DynamicCommandExecuteMethod>();
        foreach (var com in appHostConfiguration.DynamicCommands.Values)
        {
            if (appHostConfiguration.ForCommandName is null
                || (appHostConfiguration.ForCommandName is not null
                    && appHostConfiguration.ForCommandName == com.CommandName))
                selectedCommands.Add(com);
        }
        return selectedCommands;
    }

    readonly Dictionary<string, DynamicCommandExecuteMethod> _commandSpecs = new();
    public IReadOnlyDictionary<string, DynamicCommandExecuteMethod> CommandSpecs
        => _commandSpecs;

    void Add(DynamicCommandExecuteMethod commandSpec)
        => _commandSpecs.Add(commandSpec.CommandName, commandSpec);

    DynamicCommand CreateCommand(DynamicCommandExecuteMethod commandSpec)
        => new(
            commandSpec.CommandName,
            _dependencies,
            commandSpec);

#if no
    /// <summary>
    /// returns type of a command
    /// </summary>
    /// <param name="name">name of a command</param>
    /// <exception cref="ArgumentException">unknown command</exception>
    /// <returns>type of the command class</returns>
    public Type GetType(string name)
        => !_commands.TryGetValue(name, out var commandType)
            ? throw new ArgumentException(_texts._("UnknownCommand", name))
            : commandType;
#endif

#if no
    /// <summary>
    /// retourne une instance de commande à partir du nom de la commande
    /// </summary>
    /// <param name="name">nom de la commande</param>
    /// <returns>commande</returns>
    /// <exception cref="ArgumentException">unknown command</exception>
    public Command GetCommand(string name)
        => (Command)_serviceProvider
            .GetRequiredService(
                GetType(name));
#endif
}


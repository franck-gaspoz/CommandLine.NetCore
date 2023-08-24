using CommandLine.NetCore.Services.AppHost;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// set of dynamic commands operations &amp; store
/// </summary>
sealed class DynamicCommandsSet
{
    readonly Dependencies _dependencies;

    /// <summary>
    /// creates a new dynamic commands set
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    /// <param name="appHostConfiguration"></param>
    public DynamicCommandsSet(
        Dependencies dependencies,
        AppHostConfiguration appHostConfiguration)
    {
        _dependencies = dependencies;

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

    readonly Dictionary<string, DynamicCommand> _commands = new();
    public IReadOnlyDictionary<string, DynamicCommand> Commands
        => _commands;

    void Add(DynamicCommandExecuteMethod commandSpec)
        => _commandSpecs.Add(commandSpec.CommandName, commandSpec);

    DynamicCommand CreateCommand(DynamicCommandExecuteMethod commandSpec)
        => new(
            commandSpec.CommandName,
            _dependencies,
            commandSpec);

    /// <summary>
    /// try get a command by its name. if spec exits and not already created in current scope it is created and returns, else returned from cache
    /// </summary>
    /// <param name="name">name</param>
    /// <returns>comamnd or null if unknown</returns>
    public Command? TryGetCommand(string name)
    {
        if (_commands.TryGetValue(name, out var command)) return command;
        if (!_commandSpecs.TryGetValue(name, out var comSpec)) return null;
        command = CreateCommand(comSpec);
        _commands.Add(name, command);
        return command;
    }

    /// <summary>
    /// get list of commands instances
    /// </summary>
    /// <returns>list of commands instances</returns>
    public List<Command> GetCommands()
    {
        var commands = new List<Command>();
        foreach (var name in _commandSpecs.Keys)
        {
            var command = TryGetCommand(name);
            if (command is not null)
                commands.Add(command);
        }
        return commands;
    }
}


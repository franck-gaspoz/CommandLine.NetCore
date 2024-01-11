using CommandLine.NetCore.Services.AppHost;
using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// set of dynamic commands operations &amp; store
/// </summary>
sealed class DynamicCommandsSet : AbstractCommandsSetBase
{
    readonly Dependencies _dependencies;

    /// <summary>
    /// creates a new dynamic commands set
    /// </summary>
    /// <param name="texts">texts</param>
    /// <param name="serviceProvider">service provider</param>
    /// <param name="dependencies">command dependencies</param>
    /// <param name="appHostConfiguration">app host configuration</param>
    public DynamicCommandsSet(
        Texts texts,
        IServiceProvider serviceProvider,
        Dependencies dependencies,
        AppHostConfiguration appHostConfiguration)
        : base(texts, serviceProvider)
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
    static IEnumerable<DynamicCommandSpecification> GetCommands(
        AppHostConfiguration appHostConfiguration)
    {
        var selectedCommands = new List<DynamicCommandSpecification>();
        foreach (var com in appHostConfiguration.DynamicCommands.Values)
        {
            if (appHostConfiguration.ForCommandName is null
                || (appHostConfiguration.ForCommandName is not null
                    && appHostConfiguration.ForCommandName == com.CommandName))
                selectedCommands.Add(com);
        }
        return selectedCommands;
    }

    readonly Dictionary<string, DynamicCommandSpecification> _commandSpecs = new();
    public IReadOnlyDictionary<string, DynamicCommandSpecification> CommandSpecs
        => _commandSpecs;

    readonly Dictionary<string, DynamicCommand> _commands = new();
    public IReadOnlyDictionary<string, DynamicCommand> Commands
        => _commands;

    void Add(DynamicCommandSpecification commandSpec)
        => _commandSpecs.Add(
                commandSpec.CommandName,
                commandSpec);

    DynamicCommand CreateCommand(DynamicCommandSpecification commandSpec)
        => new(_dependencies, commandSpec);

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

    /// <inheritdoc/>
    public override bool Exists(string name)
        => _commandSpecs.ContainsKey(name);

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

    /// <inheritdoc/>
    public override SortedList<string, string> GetCommandNames()
    {
        var names = new SortedList<string, string>();
        foreach (var name in _commandSpecs.Select(
            x => x.Key))
            names.Add(name, name);
        return names;
    }

    /// <inheritdoc/>
    public override CommandProperties GetProperties(string name)
    {
        if (!_commandSpecs.TryGetValue(name, out var comSpec))
            throw UnknownCommand(name);

        return new CommandProperties(
            name,
            comSpec.Tags.ToArray(),
            GetDefaultNamespace(),
            ""
            );
    }

    /// <inheritdoc/>
    public string GetDefaultNamespace()
        => GetType().Namespace!;
}
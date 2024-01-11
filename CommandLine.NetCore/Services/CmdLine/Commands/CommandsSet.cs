using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// set of class &amp; dynamic commands operations &amp; store
/// </summary>
sealed class CommandsSet : AbstractCommandsSetBase
{
    readonly ClassCommandsSet _classCommandsSet;
    readonly DynamicCommandsSet _dynamicCommandsSet;

    /// <summary>
    /// Creates a new commands set
    /// </summary>
    /// <param name="texts">texts</param>
    /// <param name="classCommandsSet">class commands set</param>
    /// <param name="dynamicCommandsSet">dynamic commands set</param>
    /// <param name="serviceProvider">service provider</param>
    public CommandsSet(
        Texts texts,
        ClassCommandsSet classCommandsSet,
        DynamicCommandsSet dynamicCommandsSet,
        IServiceProvider serviceProvider
        ) : base(texts, serviceProvider)
    {
        _classCommandsSet = classCommandsSet;
        _dynamicCommandsSet = dynamicCommandsSet;
    }

    /// <summary>
    /// return a commanfd instance within the command name
    /// </summary>
    /// <param name="name">nom de la commande</param>
    /// <returns>commande</returns>
    /// <exception cref="ArgumentException">unknown command</exception>
    public Command GetCommand(string name)
    {
        var comPerType = _classCommandsSet.TryGetCommand(name);
        if (comPerType is not null) return comPerType;
        var comPerName = _dynamicCommandsSet.TryGetCommand(name);
        if (comPerName is not null) return comPerName;
        throw UnknownCommand(name);
    }

    /// <summary>
    /// get list of commands instances
    /// </summary>
    /// <returns>list of commands instances</returns>
    public List<Command> GetCommands()
    {
        var coms = new List<Command>();
        foreach (var commandType in _classCommandsSet.Commands.Values)
            coms.Add((Command)ServiceProvider.GetRequiredService(commandType));
        coms.AddRange(_dynamicCommandsSet.GetCommands());
        return coms;
    }

    /// <summary>
    /// returns list of command names
    /// </summary>
    /// <returns>list of command names</returns>
    public override SortedList<string, string> GetCommandNames()
    {
        var names = new SortedList<string, string>();
        foreach (var name in _classCommandsSet.GetCommandNames())
            names.Add(name.Key, name.Value);
        foreach (var name in _dynamicCommandsSet.GetCommandNames())
            names.Add(name.Key, name.Value);
        return names;
    }

    /// <inheritdoc/>
    public override string GetNamespace(string name)
    {
        if (_classCommandsSet.Exists(name))
            return _classCommandsSet.GetNamespace(name);
        if (_dynamicCommandsSet.Exists(name))
            return _dynamicCommandsSet.GetNamespace(name);
        throw UnknownCommand(name);
    }

    /// <summary>
    /// build dynamic commands
    /// <para>usefull to store specifications</para>
    /// </summary>
    public void BuildDynamicCommands()
        => _dynamicCommandsSet.GetCommands();

    /// <inheritdoc/>
    public override bool Exists(string name)
        => _classCommandsSet.Exists(name) || _dynamicCommandsSet.Exists(name);

    /// <inheritdoc/>
    public override List<string> GetTags(string name)
    {
        if (_classCommandsSet.Exists(name))
            return _classCommandsSet.GetTags(name);
        if (_dynamicCommandsSet.Exists(name))
            return _dynamicCommandsSet.GetTags(name);
        throw UnknownCommand(name);
    }
}


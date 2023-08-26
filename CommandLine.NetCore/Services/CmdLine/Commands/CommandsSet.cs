using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// set of class &amp; dynamic commands operations &amp; store
/// </summary>
sealed class CommandsSet
{
    readonly Texts _texts;
    readonly ClassCommandsSet _classCommandsSet;
    readonly DynamicCommandsSet _dynamicCommandsSet;
    readonly IServiceProvider _serviceProvider;

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
        )
    {
        _texts = texts;
        _classCommandsSet = classCommandsSet;
        _dynamicCommandsSet = dynamicCommandsSet;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// retourne une instance de commande à partir du nom de la commande
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
        throw new ArgumentException(_texts._("UnknownCommand", name));
    }

    /// <summary>
    /// get list of commands instances
    /// </summary>
    /// <returns>list of commands instances</returns>
    public List<Command> GetCommands()
    {
        var coms = new List<Command>();
        foreach (var commandType in _classCommandsSet.Commands.Values)
            coms.Add((Command)_serviceProvider.GetRequiredService(commandType));
        coms.AddRange(_dynamicCommandsSet.GetCommands());
        return coms;
    }



}


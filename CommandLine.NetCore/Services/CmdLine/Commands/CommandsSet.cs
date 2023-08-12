using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// set of class &amp; dynamic commands operations &amp; store
/// </summary>
sealed class CommandsSet
{
    readonly Texts _texts;
    readonly ClassCommandsSet _classCommandsSet;
    readonly DynamicCommandsSet _dynamicCommandsSet;

    /// <summary>
    /// Creates a new commands set
    /// </summary>
    /// <param name="texts">texts</param>
    /// <param name="classCommandsSet">class commands set</param>
    /// <param name="dynamicCommandsSet">dynamic commands set</param>
    public CommandsSet(
        Texts texts,
        ClassCommandsSet classCommandsSet,
        DynamicCommandsSet dynamicCommandsSet
        )
    {
        _texts = texts;
        _classCommandsSet = classCommandsSet;
        _dynamicCommandsSet = dynamicCommandsSet;
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

}


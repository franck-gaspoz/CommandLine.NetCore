using CommandLine.NetCore.Commands.CmdLine;
using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.AppHost;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// set of class commands operations &amp; store
/// </summary>
sealed class ClassCommandsSet
{
    readonly Texts _texts;
    readonly IServiceProvider _serviceProvider;

    public ClassCommandsSet(
        Texts texts,
        IServiceProvider serviceProvider,
        AssemblySet assemblySet,
        AppHostConfiguration appHostConfiguration)
    {
        _texts = texts;
        _serviceProvider = serviceProvider;

        foreach (var classType in GetCommandTypes(assemblySet, appHostConfiguration))
        {
            Add(
                Command.ClassNameToCommandName(classType.Name),
                classType);
        }
    }

    /// <summary>
    /// return selectables types from assembly set
    /// <para>if unique command, returns the unique command</para>
    /// <para>'Help' command is always added if found</para>
    /// </summary>
    /// <param name="assemblySet"></param>
    /// <param name="appHostConfiguration"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetCommandTypes(
        AssemblySet assemblySet,
        AppHostConfiguration appHostConfiguration)
    {
        var commandTypes = new List<Type>();
        foreach (var assembly in assemblySet.Assemblies)
            commandTypes
                .AddRange(
                    assembly
                        .GetTypes()
                        .Where(x => !x.IsAbstract
                            && x.InheritsFrom(typeof(Command))));

        var selectedTypes = new List<Type>();
        foreach (var type in commandTypes)
        {
            if ((appHostConfiguration.ForCommandType is null
                || (appHostConfiguration.ForCommandType is not null
                    && appHostConfiguration.ForCommandType == type)
                || (appHostConfiguration.ForCommandName is not null
                    && appHostConfiguration.ForCommandName ==
                        Command.ClassNameToCommandName(
                            type.Name))
                || type == typeof(Help)))
                selectedTypes.Add(type);
        }

        return selectedTypes;
    }

    readonly Dictionary<string, Type> _commands = new();
    public IReadOnlyDictionary<string, Type> Commands
        => _commands;

    void Add(
        string name,
        Type commandType)
        => _commands.Add(name, commandType);

    /// <summary>
    /// returns type of a command
    /// </summary>
    /// <param name="name">name of a command</param>
    /// <exception cref="ArgumentException">unknown command</exception>
    /// <returns>type of the command class</returns>
    Type GetType(string name)
        => !_commands.TryGetValue(name, out var commandType)
            ? throw new ArgumentException(_texts._("UnknownCommand", name))
            : commandType;

    /// <summary>
    /// returns type of a command
    /// </summary>
    /// <param name="name">name of a command</param>
    /// <exception cref="ArgumentException">unknown command</exception>
    /// <returns>type of the command class</returns>
    Type? TryGetType(string name)
        => _commands.TryGetValue(name, out var commandType)
            ? commandType : null;

    /// <summary>
    /// returns a instance of the command from the command name
    /// </summary>
    /// <param name="name">nom de la commande</param>
    /// <returns>commande</returns>
    /// <exception cref="ArgumentException">unknown command</exception>
    public Command GetCommand(string name)
        => (Command)_serviceProvider
            .GetRequiredService(
                GetType(name));

    /// <summary>
    /// try to returns a instance of the command from the command name, if not null returns null
    /// </summary>
    /// <param name="name">nom de la commande</param>
    /// <returns>commande</returns>
    /// <exception cref="ArgumentException">unknown command</exception>
    public Command? TryGetCommand(string name)
    {
        var type = TryGetType(name);
        if (type is null) return null;
        return (Command?)_serviceProvider
            .GetService(type);
    }
}


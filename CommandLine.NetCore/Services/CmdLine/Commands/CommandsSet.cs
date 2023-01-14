using CommandLine.NetCore.Commands.CmdLine;
using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.AppHost;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

sealed class CommandsSet
{
    readonly Texts _texts;
    readonly IServiceProvider _serviceProvider;

    public CommandsSet(
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

    public static IEnumerable<Type> GetCommandTypes(
        AssemblySet assemblySet,
        AppHostConfiguration appHostConfiguration)
    {
        var commandTypes = new List<Type>();
        foreach (var assembly in assemblySet.Assemblies)
        {
            commandTypes
                .AddRange(
                    assembly
                        .GetTypes()
                        .Where(x => !x.IsAbstract
                            && x.InheritsFrom(typeof(Command))));
        }

        var selectedTypes = new List<Type>();

        foreach (var type in commandTypes)
        {
            if ((appHostConfiguration.ForCommandType is null
                || (appHostConfiguration.ForCommandType is not null
                    && appHostConfiguration.ForCommandType == type)
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
    public Type GetType(string name)
        => !_commands.TryGetValue(name, out var commandType)
            ? throw new ArgumentException(_texts._("UnknownCommand", name))
            : commandType;

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
}


using System.Reflection;

using CommandLine.NetCore.Commands;
using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Services.CmdLine;

internal sealed class CommandsSet
{
    private readonly Texts _texts;
    private readonly IServiceProvider _serviceProvider;

    public CommandsSet(
        Texts texts,
        IServiceProvider serviceProvider)
    {
        _texts = texts;
        _serviceProvider = serviceProvider;

        foreach (var classType in GetCommandTypes())
        {
            Add(
                Command.ClassNameToCommandName(classType.Name),
                classType);
        }
    }

    public static IEnumerable<Type> GetCommandTypes()
        => Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.InheritsFrom(typeof(Command)));

    private readonly Dictionary<string, Type> _commands = new();

    public IReadOnlyDictionary<string, Type> Commands
        => _commands;

    private void Add(
        string name,
        Type commandType)
        => _commands.Add(name, commandType);

    /// <summary>
    /// returns type of a command
    /// </summary>
    /// <param name="name">name of a command</param>
    /// <exception cref="ArgumentException">unknown command</exception>
    /// <returns></returns>
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


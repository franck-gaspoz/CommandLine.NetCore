using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// a command that is declared and implemented dynamically
/// </summary>
sealed class DynamicCommand : Command
{
    readonly DynamicCommandExecuteMethod _method;

    readonly DynamicCommandContext _context;

    readonly CommandBuilder _builder;
    readonly string _name;

    public override string Name => _name;

    /// <summary>
    /// builds a new dynamic command from an execute method
    /// </summary>
    /// <param name="commandName">nom de la commande</param>
    /// <param name="dependencies">command dependencies</param>
    /// <param name="method">execute method that specify and implements the command</param>
    public DynamicCommand(
        string commandName,
        Dependencies dependencies,
        DynamicCommandExecuteMethod method)
            : base(dependencies)
    {
        _name = commandName;
        _method = method;
        _context = new(dependencies);
        _builder = new(dependencies, commandName, RunCommand);
    }

    /// <inheritdoc/>
    protected override CommandResult Execute(ArgSet args)
        => _method.Execute(args, _builder, _context);
}

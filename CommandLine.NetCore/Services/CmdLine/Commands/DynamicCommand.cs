using CommandLine.NetCore.Services.CmdLine.Commands.Attributes;
using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// a command that is declared and implemented dynamically
/// </summary>
[IgnoreCommand]
sealed class DynamicCommand : Command
{
    internal DynamicCommandSpecification Specification { get; private set; }

    readonly DynamicCommandContext _context;
    readonly CommandBuilder _builder;
    readonly string _name;

    public override string Name => _name;

    /// <summary>
    /// builds a new dynamic command from an execute method
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    /// <param name="specification">specification of syntax and implementation of the command</param>
    public DynamicCommand(
        Dependencies dependencies,
        DynamicCommandSpecification specification)
            : base(dependencies)
    {
        _name = specification.CommandName;
        Specification = specification;
        _context = new(dependencies);
        _builder = new(dependencies, _name, Specification, RunCommand);

        SyntaxMatcherDispatcher = Specification.SpecificationDelegate(
            _builder, _context);
    }

    /// <summary>
    /// returns the command name for this dynamic command
    /// </summary>
    /// <returns>command name</returns>
    public override string ClassNameToCommandName()
        => _name;

    /// <inheritdoc/>
    protected override SyntaxMatcherDispatcher Declare()
        => new(_name, Texts, Parser, GlobalSettings, Console, _context.Logger);
}

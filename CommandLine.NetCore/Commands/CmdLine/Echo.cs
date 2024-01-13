using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Commands.Attributes;
using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Commands.CmdLine;

/// <summary>
/// command line echo
/// </summary>
[Package(Packages.cmdLine)]
[Tag(Tags.cmdLine, Tags.shell)]
sealed class Echo : Command
{
    /// <inheritdoc/>
    public Echo(
        Dependencies dependencies) : base(dependencies)
    { }

    /// <inheritdoc/>
    protected override SyntaxMatcherDispatcher Declare() =>

        // Param
        For(Param())
            .Do(() => EchoImpl);

    void EchoImpl(string text)
        => Console.Out.WriteLine(text);
}

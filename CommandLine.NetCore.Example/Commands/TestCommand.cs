
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;

namespace CommandLine.NetCore.Example.Commands;

internal class TestCommand : Command
{
    public TestCommand(Dependencies dependencies) : base(dependencies) { }

    protected override CommandResult Execute(ArgSet args) =>
        For()
        .Do(() => TestCommandBody)

        .Options(
            Opt<List<string>>("a"),
            Opt("b"),
            Opt("c"),
            Opt("d"),
            Opt("e")
            )

        .With(args);

    private void TestCommandBody()
    {

    }
}


using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;

namespace CommandLine.NetCore.Example.Commands;

class TestCommand : Command
{
    public TestCommand(Dependencies dependencies) : base(dependencies) { }

    protected override CommandResult Execute(ArgSet args) =>
        For(
            Param("com"),
            Opt<List<string>>("strList"),
            Opt("flag", true),
            Opt("option", false, 2))

        .Do(() => TestCommandBody)

        .Options(
            Opt("help", true)
            )

        .With(args);

    /*
        is specyfing this command syntax:
        
        testCommand com {string},{string} --flag? [--option str1 str2] --help? 
        
        example:
        testCommand com abc,def --flag --option 1 2 --help
        
        run method arguments:
        
        TestCommandBody( 
            string com, 
            List<string> strList,
            bool flag,
            string?[] option,
            bool help) { ...
     */

    void TestCommandBody()
    {

    }
}

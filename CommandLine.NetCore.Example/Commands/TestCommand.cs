using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;

namespace CommandLine.NetCore.Example.Commands;

/// <summary>
/// test command for development,test and example purpose
/// </summary>
class TestCommand : Command
{
    public TestCommand(Dependencies dependencies) : base(dependencies) { }

    protected override CommandResult Execute(ArgSet args) =>
        For(
            Param("com"),
            Param(),
            Opt<List<string>>("strList"),
            Flag("flag", true),
            Opt("option", valueCount: 2))

        .Do(() => TestCommandBody)

        .Options(
            Flag("debug", true)
            )

        .With(args);

    /*
        is specyfing this command syntax:
        
        test-command com --strList {string},{string} --flag? [--option str1 str2] --debug? 
        
        example:
        test-command com --strList abc,def --flag --option 1 2 --debug
        
        run method arguments:
        
        notice: arguments without expected values doesn't belong to the list of parmeters

        TestCommandBody(
            List<string> strList,
            bool flag,
            string?[] option,
            bool debug) 
        { ...
     */

    void TestCommandBody(
        string str,
        List<string> strList,
        bool flag,
        List<string> option,   // pb ici: l'arg est string!
        bool debug)
    {

    }
}

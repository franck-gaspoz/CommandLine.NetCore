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
            // must not be mapped
            Param("com"),
            // must not be mapped
            Opt("0ValueOpt"),
            // List<string>,count=0 | bool
            Opt("0OptionalValueOpt", isOptional: true),
            // bool
            Flag("flag", isOptional: true),
            // List<string>?
            Opt("opts", isOptional: true, valueCount: 1),
            // string
            Param(),
            // List<List<string>>
            Opt<List<string>>("strList"),
            // List<string>
            Opt("option", valueCount: 2))

        .Do(() => TestCommandBody)

        .Options(
            // bool
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
        bool oOptionalValueOpt,
        bool flag,
        List<string>? opts,
        string param,
        List<List<string>> strList,
        List<string> option,
        bool debug)
    {

    }
}

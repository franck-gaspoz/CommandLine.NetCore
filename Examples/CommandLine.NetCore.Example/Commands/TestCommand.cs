using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Example.Commands;

/// <summary>
/// test command for development,test and example purpose
/// <para><code>test-command com --0ValueOpt --opts myOpt strValue --strList str1,str2 --option opt1 opt2 --debug --parser-logging Trace</code></para>
/// </summary>
class TestCommand : Command
{
    /// <inheritdoc/>
    public TestCommand(Dependencies dependencies) : base(dependencies) { }

    /// <inheritdoc/>
    protected override SyntaxMatcherDispatcher Declare() =>
        For(
            // must not be mapped
            Param("com"),
            // must not be mapped
            Opt("0ValueOpt"),
            // string? (value count = 1)
            Opt("opts", isOptional: true, valueCount: 1),
            // bool (value count = 0)
            Opt("0OptionalValueOpt", isOptional: true),
            // bool
            Flag("flag", isOptional: true),
            // string
            Param(),
            // List<string> (value count = 1)
            Opt<List<string>>("strList", isOptional: true),
            // List<string>
            Opt("option", valueCount: 2))

        .Do(() => TestCommandBody)

        .Options(
            // bool
            Flag("debug", true)
            );

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
        string? opts,
        bool oOptionalValueOpt,
        bool flag,
        string param,
        List<string>? strList,
        string[] option,
        bool debug)
    {

    }
}

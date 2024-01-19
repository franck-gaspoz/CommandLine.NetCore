using System;

using CommandLine.NetCore.Services.CmdLine;

using Xunit;

using static CommandLine.NetCore.Services.CmdLine.Settings.Globals;

namespace CommandLine.NetCore.Test;

/// <summary>
/// testing command line command result
/// </summary>
public class CheckCommandResultTests
{
    [Fact]
    public void Given_unknown_result_fail_with_argument_exception()
    {
        var cr = new CommandLineInterfaceBuilder()
            .Build(new string[] { "unknown-command" })
            .Run();
        Assert.NotNull(cr.Exception);
        Assert.IsType<ArgumentException>(cr.Exception);
        Assert.Equal(cr.ExitCode, ExitFail);
    }

    [Fact]
    public void Given_no_args_result_fail_with_argument_exception()
    {
        var cr = new CommandLineInterfaceBuilder()
            .Build(Array.Empty<string>())
            .Run();
        Assert.NotNull(cr.Exception);
        Assert.IsType<ArgumentException>(cr.Exception);
        Assert.Equal(cr.ExitCode, ExitFail);
    }
}

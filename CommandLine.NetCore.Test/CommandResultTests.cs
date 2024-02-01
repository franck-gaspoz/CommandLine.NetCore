using System;

using CommandLine.NetCore.Services.CmdLine;

using Xunit;

using static CommandLine.NetCore.Services.CmdLine.Settings.Globals;

namespace CommandLine.NetCore.Test;

/// <summary>
/// testing command line command result
/// </summary>
public class CommandResultTests
{
    const string CommandName = "test";

    [Fact]
    public void Given_unknown_result_fail_with_argument_exception()
    {
        var cr = new CommandLineInterfaceBuilder()
            .Build(new string[] { CommandName })
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

    [Fact]
    public void Given_no_command_operation_result_fail_with_invalid_operation_exception()
    {
        var cr = new CommandLineInterfaceBuilder()
            .AddCommand(CommandName, (builder, ctx) => builder
                .For()
                .SyntaxMatcherDispatcher)
            .Build(new string[] { CommandName })
            .Run();
        Assert.NotNull(cr.Exception);
        Assert.IsType<InvalidOperationException>(cr.Exception);
        Assert.Equal(cr.ExitCode, ExitFail);
    }

    [Fact]
    public void Given_null_delegate_result_fail_with_null_reference_exception()
    {
#pragma warning disable CS8600
#pragma warning disable CS8625
        var cr = new CommandLineInterfaceBuilder()
            .AddCommand(CommandName, (builder, ctx) => builder
                .For(builder.Param<int>())
                .Do((Action)null))
            .Build(new string[] { CommandName, "1" })
            .Run();
#pragma warning restore CS8625
#pragma warning restore CS8600
        Assert.NotNull(cr.Exception);
        Assert.IsType<NullReferenceException>(cr.Exception);
        Assert.Equal(cr.ExitCode, ExitFail);
    }
}

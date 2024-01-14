using CommandLine.NetCore.Services.CmdLine;

using Xunit;

namespace CommandLine.NetCore.Test;

public class ParserArgumentsTests
{
    const string CommandName = "test";

    static string[] GetArgs(string parameter)
        => new string[] { CommandName, parameter };

    object? CheckArgType<T>(string arg)
    {
        object? res = null;
        new CommandLineInterfaceBuilder()
            .AddCommand(CommandName, (builder, ctx) => builder
                .For(builder.Param<T>())
                    .Do((T x) => { res = x; }))
            .Build(GetArgs(arg))
            .Run();
        return res;
    }

    [Fact]
    public void Given_valid_int_argument_exec_success()
    {
        var res = CheckArgType<int>("1");
        Assert.Equal(1, (int)res!);
    }

    [Fact]
    public void Given_valid_float_argument_exec_success()
    {
        var res = CheckArgType<float>("1,5");
        Assert.Equal(1.5, (float)res!);
    }

    [Fact]
    public void Given_valid_double_argument_exec_success()
    {
        var res = CheckArgType<double>("3,14");
        Assert.Equal(3.14, (double)res!);
    }

    [Fact]
    public void Given_valid_string_argument_exec_success()
    {
        var res = CheckArgType<string>("str");
        Assert.Equal("str", (string)res!);
    }
}
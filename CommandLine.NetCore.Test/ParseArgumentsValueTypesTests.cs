using System;
using System.Collections.Generic;

using CommandLine.NetCore.Services.CmdLine;

using Xunit;

namespace CommandLine.NetCore.Test;

public class ParseArgumentsValueTypesTests
{
    const string CommandName = "test";

    static string[] GetArgs(string parameter)
        => new string[] { CommandName, parameter };

    static void CheckArgType<T>(string arg, T value)
    {
        T? res = default;
        res = RunCmdLine(arg, res);
        Assert.Equal(value, res);
    }

    static void CheckArgTypeAsStr<T>(string arg, T value)
    {
        T? res = default;
        res = RunCmdLine(arg, res);
        Assert.Equal(value!.ToString(), res?.ToString());
    }

    static T? RunCmdLine<T>(string arg, T? res)
    {
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
        => CheckArgType("1", 1);

    [Fact]
    public void Given_valid_short_argument_exec_success()
        => CheckArgType<short>("1", 1);

    [Fact]
    public void Given_valid_ulong_argument_exec_success()
        => CheckArgType<ulong>("1", 1);

    [Fact]
    public void Given_valid_uint_argument_exec_success()
        => CheckArgType<uint>("1", 1);

    [Fact]
    public void Given_valid_ushort_argument_exec_success()
        => CheckArgType<ushort>("1", 1);

    [Fact]
    public void Given_valid_long_argument_exec_success()
        => CheckArgType<long>("1", 1);

    [Fact]
    public void Given_valid_byte_argument_exec_success()
        => CheckArgType<byte>("1", 1);

    [Fact]
    public void Given_valid_sbyte_argument_exec_success()
        => CheckArgType<sbyte>("1", 1);

    [Fact]
    public void Given_valid_char_argument_exec_success()
        => CheckArgType<char>("A", 'A');

    [Fact]
    public void Given_valid_float_argument_exec_success()
        => CheckArgType("1,5", 1.5f);

    [Fact]
    public void Given_valid_double_argument_exec_success()
        => CheckArgType("3,14", 3.14d);

    [Fact]
    public void Given_valid_string_argument_exec_success()
        => CheckArgType("str", "str");

    [Fact]
    public void Given_valid_bool_true_argument_exec_success()
        => CheckArgType("true", true);

    [Fact]
    public void Given_valid_bool_false_argument_exec_success()
        => CheckArgType("false", false);

    [Fact]
    public void Given_valid_date_time_argument_exec_success()
    {
        var now = DateTime.Now;
        CheckArgTypeAsStr(now.ToString(), now);
    }

    [Fact]
    public void Given_valid_list_int_argument_exec_success()
    {
        var lst = new List<int> { 1, 2, 3 };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_short_argument_exec_success()
    {
        var lst = new List<short> { 1, 2, 3 };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_ulong_argument_exec_success()
    {
        var lst = new List<ulong> { 1, 2, 3 };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_uint_argument_exec_success()
    {
        var lst = new List<uint> { 1, 2, 3 };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_ushort_argument_exec_success()
    {
        var lst = new List<ushort> { 1, 2, 3 };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_long_argument_exec_success()
    {
        var lst = new List<long> { 1, 2, 3 };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_byte_argument_exec_success()
    {
        var lst = new List<byte> { 1, 2, 3 };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_sbyte_argument_exec_success()
    {
        var lst = new List<sbyte> { 1, 2, 3 };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_char_argument_exec_success()
    {
        var lst = new List<char> { 'A', 'B', 'C' };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_float_argument_exec_success()
    {
        var lst = new List<float> { 1.1f, 1.2f, 1.4f };
        CheckArgTypeAsStr(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_double_argument_exec_success()
    {
        var lst = new List<double> { 1.1d, 1.2d, 1.6d };
        CheckArgTypeAsStr(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_string_argument_exec_success()
    {
        var lst = new List<string> { "AV", "BC", "DE" };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_bool_argument_exec_success()
    {
        var lst = new List<bool> { true, false, true };
        CheckArgType(string.Join(',', lst), lst);
    }

    [Fact]
    public void Given_valid_list_date_time_argument_exec_success()
    {
        var now = DateTime.Now;
        var lst = new List<DateTime> { now, now, now };
        CheckArgTypeAsStr(string.Join(',', lst), lst);
    }

}
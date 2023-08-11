//#define SINGLE_COMMAND

#if SINGLE_COMMAND
using CommandLine.NetCore.Example.Commands;
#endif

using CommandLine.NetCore.Services.CmdLine;

new CommandLineInterfaceBuilder()

#if SINGLE_COMMAND
    // add this for single command mode (here: only get-info, no global help)
    .ForCommand<GetInfo>()

    // add this to avoid global help of the command line parser (not mandatory)
    .DisableGlobalHelp()
#endif

    .AddCommand(
        "add",
        (args, _, ctx) =>
            _.For(
                _.Param<double>(),
                _.Param<double>()
            )
            .Do(
                // top level statement implies can't use a local method as a lambda expression
                //() => (Func<double, double, double>)((x, y) => x + y)
                //() => (Action<double, double>)((x, y) => new A().Add(x,y))
                //() => (Action<double, double>)((x, y) => new B(x,y).Add())
                //() => (Action<double, double>)((x, y) => C.Add(x,y))
                //() => new Action<double>((x) => C.Add(x,x))
                (double x, double y) =>
                {
                    ctx.Console.Out.WriteLine($"x+y={x + y}");
                }
            )
            .With(args)
        )

    .Build(args)
    .Run();

static void Add(double x, double y) { }

record B(double x, double y) { public double Add() => x + y; }

class A { public void Add(double x, double y) { } }

static class C
{
    public static void Add(double x, double y) { }
}

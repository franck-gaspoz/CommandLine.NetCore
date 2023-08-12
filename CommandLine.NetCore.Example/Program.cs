//#define SINGLE_COMMAND_WITH_CLASS
#define SINGLE_COMMAND_CLASSLESS

#if SINGLE_COMMAND_WITH_CLASS || SINGLE_COMMAND_CLASSLESS
#endif

using CommandLine.NetCore.Services.CmdLine;

new CommandLineInterfaceBuilder()

#if SINGLE_COMMAND_WITH_CLASS
    // add this for single command mode (here: only get-info, no global help)
    .ForCommand<GetInfo>()

    // add this to avoid global help of the command line parser (not mandatory)
    .DisableGlobalHelp()
#endif

#if SINGLE_COMMAND_CLASSLESS
    // add this for single command mode (here: only get-info, no global help)
    .ForCommand("add")

    // add this to avoid global help of the command line parser (not mandatory)
    //.DisableGlobalHelp()
#endif

    .AddCommand("add", (args, builder, ctx) =>
        builder.For(builder.Param<double>(), builder.Param<double>())
            .Do((double x, double y) =>
            {
                ctx.Console.Out.WriteLine($"x+y={x + y}");
            })
            .With(args))

    //.AddCommand("add", (args, builder, ctx) => new CommandResult(Globals.ExitFail))

    .Build(args)
    .Run();

/*
 
// top level statement implies can't use a local method or an lambda with body as a lambda expression
// remaining possiblites are:    
//() => (Func<double, double, double>)((x, y) => x + y)
//() => (Action<double, double>)((x, y) => new A().Add(x,y))
//() => (Action<double, double>)((x, y) => new B(x,y).Add())
//() => (Action<double, double>)((x, y) => C.Add(x,y))
//() => new Action<double>((x) => C.Add(x,x))

record B(double x, double y) { public double Add() => x + y; }

class A { public void Add(double x, double y) { } }

static class C
{
    public static void Add(double x, double y) { }
}
*/

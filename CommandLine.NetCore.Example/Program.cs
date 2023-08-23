//#define SINGLE_COMMAND_WITH_CLASS
//#define SINGLE_COMMAND_CLASSLESS

#if SINGLE_COMMAND_WITH_CLASS || SINGLE_COMMAND_CLASSLESS
#endif

using CommandLine.NetCore.Services.CmdLine;
using CommandLine.NetCore.Services.CmdLine.Running;

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

    .AddCommand("add", (args, builder, ctx) => builder
        .For(builder.Param<string>(), builder.Param<string>(), builder.Param<string>())
            .Do((string x, string y, string z) =>
            {
                ctx.Console.Out.WriteLine($"x+y+z={x + y + z}");
            })
        .For(builder.Param<string>(), builder.Param<string>())
            .Do((string x, string y) =>
            {
                ctx.Console.Out.WriteLine($"x+y={x + y}");
            })
        //.For(builder.Param<string>(), builder.Param<string>()) // TODO: check how this is registered twice ?
        .For(builder.Param<string>())
            .Do((string x) =>
            {
                ctx.Console.Out.WriteLine($"x+x={x + x}");
            })
        .With(args))

    .AddCommand("datetime", (args, builder, ctx) => builder
        .For()
            .Do((CommandContext opContext) =>
            {
                ctx.Console.Out.WriteLine(DateTime.Now.ToString());
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

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
        .For(builder.Param<int>(), builder.Param<int>())
            .Do((int x, int y) =>
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
            .Do((CommandContext com) =>
            {
                com.Console.Out.WriteLine("(f=yellow,uon)current date/time:(tdoff,b=black) (b=magenta)" + DateTime.Now.ToString());
            })
        .With(args))

    //.AddCommand("add", (args, builder, ctx) => new CommandResult(Globals.ExitFail))

    .Build(args)
    .Run();

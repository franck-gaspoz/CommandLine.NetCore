//#define SINGLE_COMMAND_WITH_CLASS
//#define SINGLE_COMMAND_CLASSLESS

#if SINGLE_COMMAND_WITH_CLASS || SINGLE_COMMAND_CLASSLESS
#endif

using CommandLine.NetCore.Services.CmdLine;
using CommandLine.NetCore.Services.CmdLine.Commands.Attributes;
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

    .AddCommand("add", (builder, ctx) => builder

        .Help("add operator")
        .Package(Packages.miscelaneous)
        .Tag(Tags.math, Tags.text)

        .For(builder.Param<int>(), builder.Param<int>(), builder.Param<int>())
            .Help("x y z", "output x+y+z")
            .Do((int x, int y, int z) =>
                ctx.Console.Out.WriteLine($"{x + y + z}"))

        .For(builder.Param<int>(), builder.Param<int>())
            .Help("x y", "output x+y")
            .Do((int x, int y) =>
                ctx.Console.Out.WriteLine($"{x + y}"))

        .For(builder.Param<int>())
            .Help("x", "output x+x")
            .Do((int x) =>
                ctx.Console.Out.WriteLine($"{x + x}"))

        .For(builder.Param<string>(), builder.Param<string>())
            .Help("x y", "adds the two strings x and y")
            .Do((string x, string y)
                => ctx.Console.Out.WriteLine(x + y))
    )

    .AddCommand("datetime", (builder, ctx) => builder
        .Help("get datetime")
        .Tag(Tags.system, Tags.dateTime)
        .For()
            .Do((CommandContext com) =>
                com.Console.Out.WriteLine("(f=yellow,uon)current date/time:(tdoff) (b=magenta)" + DateTime.Now.ToString())))

    .Build(args)
    .Run();

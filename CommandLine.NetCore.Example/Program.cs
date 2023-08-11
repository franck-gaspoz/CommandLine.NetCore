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
                () => { }
            )
            .With(args)
        )

    .Build(args)
    .Run();


//void Add(double x, double y, CommandContext ctx) { }
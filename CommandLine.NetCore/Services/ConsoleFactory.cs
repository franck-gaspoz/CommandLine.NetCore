using CommandLine.NetCore.GlobalOpts;
using CommandLine.NetCore.Services.CmdLine.Settings;

using Microsoft.Extensions.DependencyInjection;

using cons = AnsiVtConsole.NetCore;

namespace CommandLine.NetCore.Services;

/// <summary>
/// console factory
/// </summary>
sealed class ConsoleFactory
{
    readonly IServiceProvider _serviceProvider;

    public ConsoleFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public ConsoleFactory Configure()
    {
        var globalSettings = _serviceProvider.GetRequiredService<GlobalSettings>();
        var console = _serviceProvider.GetRequiredService<cons.IAnsiVtConsole>();

        Configure(globalSettings, console);
        return this;
    }

    public cons.IAnsiVtConsole Create()
    {
        var globalSettings = _serviceProvider.GetRequiredService<GlobalSettings>();
        var console = new cons.AnsiVtConsole();

        Configure(globalSettings, console);
        return console;
    }

    static void Configure(GlobalSettings globalSettings, cons.IAnsiVtConsole console)
    {
        console.Out.IsMute = globalSettings
                    .SettedGlobalOptsSet
                    .Contains<S>();

        console.Settings.IsMarkupDisabled
            = globalSettings
                .SettedGlobalOptsSet
                .Contains<NoColor>();
    }
}

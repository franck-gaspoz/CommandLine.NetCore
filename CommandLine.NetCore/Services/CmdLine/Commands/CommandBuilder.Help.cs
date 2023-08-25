using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// command builder
/// </summary>
public sealed partial class CommandBuilder
{

#if no
    /// <summary>
    /// add a short description for the command syntax and the culture    
    /// </summary>
    /// <param name="text">text of the short description</param>
    /// <param name="culture">culture of the text. if null use the current culture</param>
    /// <returns>this object</returns>
    public CommandBuilder Description(string text, string? culture = null)
    {
        _configuration.Set(
            ShortDescriptionKey(_commandName),
            text,
            culture);
        return this;
    }
#endif

    void AddHelpAboutCommandSyntax(SyntaxMatcherDispatcher syntaxMatcherDispatcher)
        => syntaxMatcherDispatcher
            .For(
                Opt("h"))
                    .Do(HelpAboutCommandSyntax)
            .Options(Opt("v"), Opt("info"));

    CommandLineResult HelpAboutCommandSyntax(CommandContext context)
    {
        var args =
            new List<string>{
                "help" ,
                _commandName };

        if (context.OptSet is not null)
        {
            foreach (var opt in context.OptSet.Opts)
            {
                if (opt.IsSet)
                    args.AddRange(opt.ToArgs());
            }
        }

        foreach (var (_, globalArgSyntax) in _globalSettings
            .SettedGlobalOptsSet
            .OptSpecs)
        {
            args.AddRange(globalArgSyntax);
        }

        return _runMethod!(args.ToArray());
    }
}

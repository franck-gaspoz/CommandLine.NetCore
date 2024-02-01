using CommandLine.NetCore.Services.CmdLine.Commands.Attributes;
using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// command builder
/// </summary>
public sealed partial class CommandBuilder
{
    /// <summary>
    /// add a short description for the command syntax and the culture    
    /// </summary>
    /// <param name="text">text of the short description</param>
    /// <param name="culture">culture of the text. if null use the current culture</param>
    /// <returns>this object</returns>
    public CommandBuilder Help(string text, string? culture = null)
    {
        HelpBuilder.SetShortDescription(
            _configuration,
            _commandName,
            text,
            culture);
        return this;
    }

    /// <summary>
    /// associate the command to one or several tags
    /// </summary>
    /// <param name="tags">tags</param>
    /// <returns>this object</returns>
    public CommandBuilder Tag(params string[] tags)
    {
        DynamicCommandSpecification?.Tags
            .AddRange(tags);
        return this;
    }

    /// <summary>
    /// setup the package the command belongs to
    /// <para>does nothing if DynamicCommandSpecification is null</para>
    /// </summary>
    /// <param name="package">package</param>
    /// <returns>this object</returns>
    public CommandBuilder Package(string package)
    {
        if (DynamicCommandSpecification is not null)
            DynamicCommandSpecification!.Package = package;
        return this;
    }

    /// <summary>
    /// setup the package the command belongs to
    /// <para>does nothing if DynamicCommandSpecification is null</para>
    /// </summary>
    /// <param name="package">package</param>
    /// <returns>this object</returns>
    public CommandBuilder Package(Packages package)
    {
        if (DynamicCommandSpecification is not null)
            DynamicCommandSpecification!.Package = package.ToString();
        return this;
    }

    /// <summary>
    /// associate a dynamic command to one or several tags
    /// </summary>
    /// <param name="tags">tags</param>
    /// <returns>this object</returns>
    public CommandBuilder Tag(params object[] tags)
    {
        DynamicCommandSpecification?.Tags
            .AddRange(
                tags.Select(
                    x => x.ToString()!));
        return this;
    }

    void AddHelpAboutCommandSyntax(SyntaxMatcherDispatcher syntaxMatcherDispatcher)
        => syntaxMatcherDispatcher
            .For(
                Opt("h"))
                    .Do(HelpAboutCommandSyntax)
            .Options(Opt("v"), Opt("info"));

    CommandResult HelpAboutCommandSyntax(CommandContext context)
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

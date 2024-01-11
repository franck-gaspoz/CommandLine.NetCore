using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// commands set base
/// </summary>
abstract class AbstractCommandsSetBase : ICommandsSpecificationSet
{
    protected readonly Texts Texts;
    protected readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="texts">texts service</param>
    /// <param name="serviceProvider">service provider</param>
    public AbstractCommandsSetBase(
        Texts texts,
        IServiceProvider serviceProvider)
    {
        Texts = texts;
        ServiceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public abstract bool Exists(string name);

    /// <inheritdoc/>
    public abstract SortedList<string, string> GetCommandNames();

    /// <inheritdoc/>
    public abstract CommandProperties GetProperties(string name);

    /// <summary>
    /// build an unknwon command exception
    /// </summary>
    /// <param name="commandName">command name</param>
    /// <returns>unknwown command exception</returns>
    protected Exception UnknownCommand(string commandName)
        => new ArgumentException(Texts._("UnknownCommand", commandName));
}

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// interface of set of commands specification
/// </summary>
interface ICommandsSpecificationSet
{
    /// <summary>
    /// get command properties
    /// </summary>
    /// <param name="name">command name</param>
    /// <exception cref="ArgumentException">unknwown command exception</exception>
    /// <returns>command properties</returns>    
    public CommandProperties GetProperties(string name);

    /// <summary>
    /// check if a command with provided name exists or not
    /// </summary>
    /// <param name="name">command name</param>
    /// <returns>true if the command exists, false otherwise</returns>
    public bool Exists(string name);

    /// <summary>
    /// returns list of command names
    /// </summary>
    /// <returns>list of command names</returns>
    public SortedList<string, string> GetCommandNames();
}

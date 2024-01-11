namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// interface of set of commands specification
/// </summary>
interface ICommandsSpecificationSet
{
    /// <summary>
    /// get tags of the command with the provided name
    /// </summary>
    /// <param name="name">command name</param>
    /// <exception cref="ArgumentException">unknwown command exception</exception>
    /// <returns>tags list</returns>
    public List<string> GetTags(string name);

    /// <summary>
    /// get namespace of the command having the provided name
    /// </summary>
    /// <param name="name">command name</param>
    /// <returns>namespace of the command</returns>
    public string GetNamespace(string name);

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

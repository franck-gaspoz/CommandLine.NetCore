namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// help properties of a command
/// </summary>
class CommandHelp
{
    public string ShortDescription { get; set; }

    public string LongDescription { get; set; }

    public CommandHelp(
        string shortDescription,
        string longDescription)
    {
        ShortDescription = shortDescription;
        LongDescription = longDescription;
    }
}

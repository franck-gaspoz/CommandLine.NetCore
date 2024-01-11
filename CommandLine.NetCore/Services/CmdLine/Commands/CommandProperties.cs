namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// common properties of commands, such meta data
/// </summary>
/// <param name="Name"> name </param>
/// <param name="Tags"> tags </param>
/// <param name="Namespace"> namespace </param>
/// <param name="Package"> package </param>
public sealed record CommandProperties(
    string Name,
    string[] Tags,
    string Namespace,
    string Package);

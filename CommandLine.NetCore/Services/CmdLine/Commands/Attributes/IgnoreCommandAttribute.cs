namespace CommandLine.NetCore.Services.CmdLine.Commands.Attributes;

/// <summary>
/// this attribute put on top of a command class will prevent it to be handled by the commands loader
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IgnoreCommandAttribute : Attribute
{

}

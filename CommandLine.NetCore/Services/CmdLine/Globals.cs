namespace CommandLine.NetCore.Services.CmdLine;

internal static class Globals
{
    public const int ExitOk = 0;
    public const int ExitFail = -1;

    public const string ShortArgNamePrefix = "-";
    public const string LongArgNamePrefix = "--";

    public const string ConfigFileCoreName = ".core";
    public const string ConfigFilePrefix = "config/appSettings";
    public const string ConfigFilePostfix = ".json";

    public const string SettingsDateFormat = "dd/MM/yyyy";

    public const string GlobalArgPostFix = "GlobalArg";
}


namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// command line core globals
/// </summary>
public static class Globals
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

    /// <summary>
    /// values separarator in parameter of type list/collection
    /// </summary>
    public const char ParameterTypeListValuesSeparator = ',';

    /// <summary>
    /// add a flag value (value prefix) in parameter of type enum flag
    /// </summary>
    public const char ParameterTypeFlagEnumValuePrefixEnabled = '+';

    /// <summary>
    /// remove a flag value (value prefix) in parameter of type enum flag
    /// </summary>
    public const char ParameterTypeFlagEnumValuePrefixDisabled = '-';

    /// <summary>
    /// any prefix for flag value (value prefix) in parameter of type enum flag
    /// </summary>
    public static readonly List<char> ParameterTypeFlagEnumValuePrefixs = new()
    { ParameterTypeFlagEnumValuePrefixEnabled, ParameterTypeFlagEnumValuePrefixDisabled };

}


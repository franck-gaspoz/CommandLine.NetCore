namespace CommandLine.NetCore.Services.CmdLine.Settings;

/// <summary>
/// command line core globals
/// </summary>
public static class Globals
{
    /// <summary>
    /// exit code Ok
    /// </summary>
    public const int ExitOk = 0;

    /// <summary>
    /// exit code fail
    /// </summary>
    public const int ExitFail = -1;

    /// <summary>
    /// prefix of a short name
    /// </summary>
    public const string ShortArgNamePrefix = "-";

    /// <summary>
    /// prefix of a long name
    /// </summary>
    public const string LongArgNamePrefix = "--";

    /// <summary>
    /// core configuration file name postfix
    /// </summary>
    public const string ConfigFileCoreName = ".core";

    /// <summary>
    /// configuration file prefix
    /// </summary>
    public const string ConfigFilePrefix = "config/appSettings";

    /// <summary>
    /// configuration file postfix
    /// </summary>
    public const string ConfigFilePostfix = ".json";

    /// <summary>
    /// date format of dates in settings
    /// </summary>
    public const string SettingsDateFormat = "MM/dd/yyyy";

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
    {
        ParameterTypeFlagEnumValuePrefixEnabled,
        ParameterTypeFlagEnumValuePrefixDisabled
    };

}


namespace CommandLine.NetCore.Services.Error;

/// <summary>
/// map of error data text
/// </summary>
public sealed class ErrorDataTextMap
{
    /// <summary>
    /// funcs that returns data names of parameter number in error text (if any)
    /// </summary>
    public Func<string>[] DataText { get; private set; }

    /// <summary>
    /// creates a new map from the given mapping functions
    /// </summary>
    /// <param name="funcs">mapping functions, returns the attempted data name at array index</param>
    public ErrorDataTextMap(params Func<string>[] funcs)
        => DataText = funcs.ToArray();

}

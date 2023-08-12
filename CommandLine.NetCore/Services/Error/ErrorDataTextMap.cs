namespace CommandLine.NetCore.Services.Error;

/// <summary>
/// map of error data text
/// </summary>
public sealed class ErrorDataTextMap
{
    /// <summary>
    /// funcs that returns data names of parameter number in error text (if any)
    /// </summary>
    public string[] DataName { get; private set; }

    /// <summary>
    /// creates a new map from the given data names
    /// </summary>
    /// <param name="dataNames">mapping names, returns the attempted data name at array index</param>
    public ErrorDataTextMap(params string[] dataNames)
        => DataName = dataNames.ToArray();
}

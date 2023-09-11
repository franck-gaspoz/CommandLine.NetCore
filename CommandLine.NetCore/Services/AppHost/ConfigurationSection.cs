using System.Diagnostics;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.AppHost;

/// <summary>
/// configuration section
/// </summary>
[DebuggerDisplay("path={Path} value={Value}")]
public sealed class ConfigurationSection : Configuration, IConfigurationSection
{
    /// <inheritdoc/>
    public string Key { get; }

    /// <inheritdoc/>
    public string Path { get; }

    /// <inheritdoc/>
    public string? Value { get; set; }

    /// <summary>
    /// configuration root
    /// </summary>
    public Configuration Configuration { get; }

    /// <summary>
    /// sub sections
    /// </summary>
    List<IConfigurationSection>? _childrens { get; set; }

    /// <summary>
    /// build a new configuration section
    /// </summary>
    /// <param name="configuration">configuration root</param>
    /// <param name="key">section key</param>
    /// <param name="path">section path</param>
    /// <param name="value">value</param>
    /// <param name="subSections">sub-sections</param>
    public ConfigurationSection(
        Configuration configuration,
        string key,
        string path,
        string? value,
        IEnumerable<IConfigurationSection>? subSections = null) : base(configuration.HostConfiguration)
    {
        Configuration = configuration;
        Key = key;
        Path = path;
        Value = value;
        if (subSections != null)
        {
            _childrens = new();
            _childrens.AddRange(
                subSections.ToList());
        }
    }

    /// <summary>
    /// gets the immediate descendant configuration sub-sections
    /// </summary>
    /// <returns>configuration sub-sections</returns>
    public new IEnumerable<IConfigurationSection> GetChildren()
        => _childrens ?? new();
}

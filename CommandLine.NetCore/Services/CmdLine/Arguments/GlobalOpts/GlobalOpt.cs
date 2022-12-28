
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;

/// <summary>
/// global option
/// </summary>
public abstract class GlobalOpt : Opt
{
    public GlobalOpt(
            IConfiguration config,
            Texts texts,
            ValueConverter valueConverter)
                : base(
                    string.Empty,
                    config,
                    texts,
                    valueConverter,
                    true)
        => Name = Parser.ClassNameToOptName(
                GetType().Name);
}

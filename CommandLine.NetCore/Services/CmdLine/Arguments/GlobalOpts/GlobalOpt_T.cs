
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;

/// <summary>
/// global option
/// </summary>
public abstract class GlobalOpt<T> : Opt<T>, IGlobalOpt
{
    public GlobalOpt(
            IConfiguration config,
            Texts texts,
            ValueConverter valueConverter,
            int valuesCount = 0)
                : base(
                    string.Empty,
                    config,
                    texts,
                    valueConverter,
                    true,
                    valuesCount)
        => Name = Parser.ClassNameToOptName(
                GetType().Name);
}

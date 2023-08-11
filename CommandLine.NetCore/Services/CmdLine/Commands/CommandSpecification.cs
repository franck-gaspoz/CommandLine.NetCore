using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

public sealed class CommandSpecification : Command
{
    public CommandSpecification() : base(new Dependencies())

    /// <inheritdoc/>
    protected override CommandResult Execute(ArgSet args) 
        => throw new NotImplementedException();
}

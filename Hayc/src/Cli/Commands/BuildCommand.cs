using Hayc.Cli.Settings;
using Hayc.Common;
using Spectre.Cli;

namespace Hayc.Cli.Commands;

public sealed class BuildCommand : Command<BuildSettings>
{
    public override int Execute(CommandContext context, BuildSettings settings)
    {
        BuildEngine buildEngine = new(settings);
        
        return buildEngine.Build();
    }

    
}
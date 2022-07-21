using HaycCli.Settings;
using Spectre.Cli;
using Spectre.Console;

namespace HaycCli.Commands;

public sealed class BuildCommand : Command<BuildSettings>
{
    public override int Execute(CommandContext context, BuildSettings settings)
    {
        
        return 0;
    }
}
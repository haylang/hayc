using HaycLib.Cli.Settings;
using Spectre.Cli;
using Spectre.Console;

namespace HaycLib.Cli.Commands;

public sealed class BuildCommand : Command<BuildSettings>
{
    public override int Execute(CommandContext context, BuildSettings settings)
    {
        string response = AnsiConsole.Ask("Are you sure you want to build?", "y");
        AnsiConsole.MarkupLine($"Your response was: [red]{response}[/]");
        return 0;
    }
}
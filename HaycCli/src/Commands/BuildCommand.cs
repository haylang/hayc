using HaycCli.Settings;
using HaycCli.Utils;
using HaycLib;
using HaycLib.Location;
using HaycLib.Reporting;
using Spectre.Cli;
using Spectre.Console;

namespace HaycCli.Commands;

public sealed class BuildCommand : Command<BuildSettings>
{
    public override int Execute(CommandContext context, BuildSettings settings)
    {
        BuildEngine buildEngine = new(settings.ProjectPath);
        bool success = buildEngine.Build();

        // Print error messages
        MessagePrinter.PrintMessages(buildEngine.MessageBatch.Messages);

        if (!success)
        {
            AnsiConsole.MarkupLine("[red]Compilation failed.[/]");
            return 255;
        }
        
        AnsiConsole.MarkupLine("[lime]Compilation successful.[/]");

        return 0;
    }

    
}
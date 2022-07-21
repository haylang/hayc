using HaycCli.Settings;
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
        foreach (Message message in buildEngine.MessageBatch.Messages)
        {
            string severityColor = message.Severity switch
            {
                MessageSeverity.Info    => "white",
                MessageSeverity.Warning => "yellow",
                MessageSeverity.Error   => "red",
                _                       => throw new ArgumentOutOfRangeException()
            };

            AnsiConsole.MarkupInterpolated($"[[HAY{(int)message.Id:0000}]] [{severityColor}]{message.Severity}[/] in ");
            AnsiConsole.MarkupLine($"{GetMarkupFileLocationString(message.Location, settings.ProjectPath)}");
            AnsiConsole.MarkupLineInterpolated($"    : [cyan]{message.Content}[/]");
        }

        if (!success)
        {
            AnsiConsole.MarkupLineInterpolated($"Compilation failed.");
        }

        return 0;
    }

    private static string GetMarkupFileLocationString(FileLocation fileLocation, string projectPath)
    {
        string usePath = fileLocation.Path.StartsWith(projectPath)
                             ? fileLocation.Path[projectPath.Length..]
                             : projectPath;

        string location = fileLocation.Range.Start == fileLocation.Range.End
                              ? $"at [yellow]Ln {fileLocation.Range.Start.Line} Col {fileLocation.Range.Start.Column}[/]"
                              : $"from [yellow]Ln {fileLocation.Range.Start.Line} Col {fileLocation.Range.Start.Column}[/]"
                                + $" to [yellow]Ln {fileLocation.Range.End.Line} Col {fileLocation.Range.End.Column}[/]";

        return $"[yellow]{usePath}[/] {location}";
    }
}
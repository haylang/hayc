using HaycLib.Location;
using HaycLib.Reporting;
using Spectre.Console;

namespace HaycCli.Utils;

/// <summary>
/// Prints messages to the console.
/// </summary>
public static class MessagePrinter
{
    public static void PrintMessages(IEnumerable<Message> source)
    {
        foreach (Message message in source)
        {
            string severityColor = message.Severity switch
            {
                MessageSeverity.Info    => "white",
                MessageSeverity.Warning => "yellow",
                MessageSeverity.Error   => "red",
                _                       => throw new ArgumentOutOfRangeException()
            };

            string path = message.Location.Path;
            Position start = message.Location.Range.Start;
            
            AnsiConsole.MarkupLineInterpolated($"[{severityColor}]{message.Severity}[/]: {message.Content}");
            AnsiConsole.MarkupInterpolated($"   [i][silver]in[/] {path}[/] ");
            AnsiConsole.MarkupInterpolated($"[i][silver]at[/] [yellow]{start.Line}[/]:[yellow]{start.Column}[/][/]");
            AnsiConsole.WriteLine();
        }
    }
}
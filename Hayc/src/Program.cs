using Hayc.Cli.Commands;
using Spectre.Cli;
using Spectre.Console;

namespace Hayc;

internal static class Program
{
    public static int Main(string[] args)
    {
        CommandApp app = new();

        app.Configure(
            config =>
            {
                // Commands
                config.AddCommand<BuildCommand>("build")
                      .WithDescription("Builds the project.");

                // Output options
                config.Settings.ApplicationName = "hayc";

                // Exceptions
                config.PropagateExceptions();
            }
        );

        try
        {
            return app.Run(args);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("---");
            AnsiConsole.WriteLine();

            AnsiConsole.WriteException(ex);

            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[red]The Hay compiler crashed.[/]");
            AnsiConsole.MarkupLine(
                "This is most likely a bug in the compiler. Please report this issue to the [bold]GitHub[/] repository."
            );
            AnsiConsole.MarkupLine("[yellow]The repository can be found here:[/] https://github.com/haylang/hayc");
            AnsiConsole.WriteLine();

            return -1;
        }
    }
}
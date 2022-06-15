using Spectre.Cli;

namespace Hayc.Cli.Settings;

public class BuildSettings : CommandSettings
{
    [CommandArgument(0, "[PROJECT_PATH]")]
    public string ProjectPath { get; set; } = String.Empty;
}
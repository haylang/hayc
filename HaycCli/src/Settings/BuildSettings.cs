using Spectre.Cli;

namespace HaycCli.Settings;

public class BuildSettings : CommandSettings
{
    [CommandArgument(0, "[PROJECT_PATH]")]
    public string ProjectPath { get; set; } = ".";
}
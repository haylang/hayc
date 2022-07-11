using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Cli;

namespace Hayc.Cli.Settings;

public class BuildSettings : CommandSettings
{
    [Description("The path to the project to build.")]
    [CommandArgument(0, "<PROJECT_PATH>")]
    public string ProjectPath { get; init; } = String.Empty;

    [Description("Prints more information about the build.")]
    [CommandOption("-v|--verbose")]
    public bool Verbose { get; init; } = false;

    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    public override ValidationResult Validate()
    {
        if (!Directory.Exists(ProjectPath))
        {
            return ValidationResult.Error("Project directory not found.");
        }
        
        return base.Validate();
    }
}
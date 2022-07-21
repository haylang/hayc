using Spectre.Cli;

namespace HaycCli.Settings;

public class BuildSettings : CommandSettings
{
    [CommandOption("--projectPath")]
    public string ProjectPath { get; set; } = ".";

    public override ValidationResult Validate()
    {
        ProjectPath = Path.GetFullPath(ProjectPath);
        
        if (!Directory.Exists(ProjectPath))
        {
            return ValidationResult.Error("The given project directory does not exist.");
        }
        
        return base.Validate();
    }
}
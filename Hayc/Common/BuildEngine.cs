using Hayc.Cli.Settings;
using Spectre.Console;

namespace Hayc.Common;

/// <summary>
/// Parses, analyzes and compiles Hay source files.
/// </summary>
public sealed class BuildEngine
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="buildSettings">The settings to use for this build.</param>
    public BuildEngine(BuildSettings buildSettings)
    {
        CompileMessages = new MessageBatch();

        _buildSettings = buildSettings;
        _sourceFiles   = new List<string>();
    }

    /// <summary>
    /// The messages from the compilation.
    /// </summary>
    public MessageBatch CompileMessages { get; }

    /// <summary>
    /// The build settings.
    /// </summary>
    private readonly BuildSettings _buildSettings;

    /// <summary>
    /// The paths of the source files.
    /// </summary>
    private readonly List<string> _sourceFiles;

    /// <summary>
    /// Builds the source files.
    /// </summary>
    /// <returns>The exit code for the compiler.</returns>
    public int Build()
    {
        foreach (string file in Directory.GetFiles(_buildSettings.ProjectPath, "*.hay", SearchOption.AllDirectories))
        {
            _sourceFiles.Add(file);
        }

        foreach (Message message in CompileMessages.Messages)
        {
            AnsiConsole.MarkupLineInterpolated(
                $"[blue]{message.Location.ToString()[_buildSettings.ProjectPath.Length..]}[/] {message.Content}"
            );
        }

        return CompileMessages.AnyOfSeverity(MessageSeverity.Error) ? -1 : 0;
    }
}
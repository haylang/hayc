using Antlr4.Runtime;
using HaycLib.Antlr;
using HaycLib.Reporting;

namespace HaycLib;

/// <summary>
/// Compiles source files from a directory.
/// </summary>
public sealed class BuildEngine
{
    public BuildEngine(string projectPath)
    {
        _projectPath = projectPath;

        MessageBatch = new MessageBatch();
    }

    /// <summary>
    /// The messages from the compilation.
    /// </summary>
    public MessageBatch MessageBatch { get; }

    /// <summary>
    /// The path of the project.
    /// </summary>
    private readonly string _projectPath;

    /// <summary>
    /// Builds the source files.
    /// </summary>
    public bool Build()
    {
        // The paths to each source file
        string[] filePaths = Directory.GetFiles(_projectPath, "*.hay", SearchOption.AllDirectories);

        /*
         * Compilation:
         * 1. Invoke ANTLR
         * 3. Semantic analysis
         * 4. Output files
         */

        IEnumerable<HayParser.FileContext> parsed = Parse(filePaths);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (MessageBatch.Messages.Any(x => x.Severity == MessageSeverity.Error))
        {
            return false;
        }

        // Invoke LLVM in the future here!

        return true;
    }

    /// <summary>
    /// Performs syntactic analysis on the given files.
    /// </summary>
    /// <param name="filePaths">The paths of the files to lex.</param>
    /// <returns>In the array, each index corresponds to the index in filePaths.</returns>
    public IEnumerable<HayParser.FileContext> Parse(string[] filePaths)
    {
        HayParser.FileContext[] parsed = new HayParser.FileContext[filePaths.Length];

        for (int i = 0; i < filePaths.Length; ++i)
        {
            // The path to the file.
            string path = filePaths[i];

            // Lexing
            ICharStream stream = new AntlrFileStream(path);
            ITokenSource lexer = new HayLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            // Parsing
            HayParser parser = new(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new MessageErrorListener(path, MessageBatch));
            HayParser.FileContext tree = parser.file();

            // We're done!
            parsed[i] = tree;
        }

        return parsed;
    }
}
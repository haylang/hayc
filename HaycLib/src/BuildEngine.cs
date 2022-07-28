using HaycLib.Ast.Nodes;
using HaycLib.Lexing;
using HaycLib.Parsing;
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

        IEnumerable<FileNode> parsed = Parse(filePaths);

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
    public IEnumerable<FileNode> Parse(string[] filePaths)
    {
        List<FileNode> parsed = new(filePaths.Length);

        for (int i = 0; i < filePaths.Length; ++i)
        {
            // The path to the file.
            string path = filePaths[i];
            string content = File.ReadAllText(path);

            Tokenizer tokenizer = new(path, content);
            List<Token> tokens = tokenizer.Tokenize().ToList();

            IEnumerable<Token> invalidTokens = tokens.Where(x => x.Type == TokenType.Invalid).ToList();
            if (invalidTokens.Any())
            {
                foreach (Token token in invalidTokens)
                {
                    MessageBatch.AddError("Invalid token.", token.Location);
                    tokens.Remove(token);
                }
            }

            Parser parser = new(tokens);
            FileNode? node = parser.Parse();
            MessageBatch.AddRange(parser.GetMessages());

            // We're done!
            if (node != null)
            {
                parsed.Add(node);
            }
        }

        return parsed;
    }
}
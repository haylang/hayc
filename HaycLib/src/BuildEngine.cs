using HaycLib.Ast;
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

        CompilationMessages = new MessageBatch();
    }

    /// <summary>
    /// The messages from the compilation.
    /// </summary>
    public MessageBatch CompilationMessages { get; }
    
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
         * 1. Lex
         * 2. Parse
         * 3. Semantic analysis
         * 4. Output files
         */

        Token[][] lexed = Lex(filePaths);
        IEnumerable<FileNode> parsed = Parse(filePaths, lexed);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (CompilationMessages.Messages.Any(x => x.Severity == MessageSeverity.Error))
        {
            return false;
        }

        // Invoke LLVM in the future here!
        
        return true;
    }

    /// <summary>
    /// Performs lexical analysis on the given files.
    /// </summary>
    /// <param name="filePaths">The paths of the files to lex.</param>
    /// <returns>In the parent array, each index corresponds to the index in filePaths.</returns>
    public Token[][] Lex(string[] filePaths)
    {
        Token[][] lexed = new Token[filePaths.Length][];

        for (int i = 0; i < lexed.Length; ++i)
        {
            string filePath = filePaths[i];
            string fileContent = File.ReadAllText(filePath);

            Lexer lexer = new(CompilationMessages, filePath, fileContent);
            Token[] tokens = lexer.Lex();
            lexed[i] = tokens;
        }

        return lexed;
    }

    /// <summary>
    /// Performs syntactic analysis on the given files.
    /// </summary>
    /// <param name="filePaths">The paths of the files to lex.</param>
    /// <param name="lexed">The tokens of each file.</param>
    /// <returns>In the array, each index corresponds to the index in filePaths.</returns>
    public IEnumerable<FileNode> Parse(string[] filePaths, Token[][] lexed)
    {
        FileNode[] parsed = new FileNode[filePaths.Length];

        for (int i = 0; i < filePaths.Length; ++i)
        {
            Token[] tokens = lexed[i];
            Parser parser = new Parser(CompilationMessages, tokens);
            parsed[i] = parser.Parse();
        }

        return parsed;
    }
}
using HaycLib.Ast.DataObjects;
using HaycLib.Lexing;

namespace HaycLib.Parsing;

public static class ParsingExtensions
{
    public static void ErrorUnwantedModifiers(
        this ParserState state,
        IEnumerable<Token> givenModifiers,
        params string[] allowedModifiers)
    {
        foreach (Token modifier in givenModifiers)
        {
            if (modifier.Type != TokenType.Modifier)
            {
                throw new InvalidOperationException("Only modifier tokens are allowed.");
            }

            if (allowedModifiers.Contains(modifier.Value.ToString()))
            {
                continue;
            }

            state.Error($"The modifier {modifier.Value} is not applicable here.", modifier);
        }
    }

    public static IEnumerable<Token> CollectModifiers(this ParserState state)
    {
        List<Token> modifiers = new(0);
        while (state.CurrentTokenIs(TokenType.Modifier))
        {
            modifiers.Add(state.CurrentToken);
            state.NextToken();
        }
        return modifiers;
    }

    public static void SkipAttributes(this ParserState state)
    {
        while (state.CurrentTokenIs(TokenType.LeftBracket))
        {
            state.NextToken(); // [

            if (!state.CurrentTokenIs(TokenType.Identifier))
            {
                continue;
            }
            
            state.NextToken(); // attribute name

            if (!state.CurrentTokenIs(TokenType.RightBracket))
            {
                continue;
            }
            
            state.NextToken(); // ]
        }
    }

    public static void Recover(this ParserState state, ParseRecoverMode mode)
    {
        if (mode == ParseRecoverMode.Detect)
        {
            state.BeginNewSnapshot();
            // If within the next 10 tokens we find an opening brace, we'll use
            // UntilBrace; otherwise UntilSemicolon. The number 10 is arbitrary
            // and picked as a compromise between speed and good reporting.

            mode = ParseRecoverMode.UntilSemicolon;
            
            for (int i = 0; i < 10; ++i)
            {
                if (state.CurrentTokenIs(TokenType.LeftBrace))
                {
                    mode = ParseRecoverMode.UntilBrace;
                    break;
                }
                
                state.NextToken();
            }
            
            state.RevertToPreviousSnapshot();
        }

        switch (mode)
        {
            case ParseRecoverMode.UntilBrace:
                int openBraces = 0;
                while (true)
                {
                    if (state.CurrentTokenIs(TokenType.LeftBrace))
                    {
                        ++openBraces;
                    }
                    else if (state.CurrentTokenIs(TokenType.RightBrace))
                    {
                        --openBraces;

                        if (openBraces <= 0)
                        {
                            return;
                        }
                    }
                    
                    state.NextToken();
                }
            
            case ParseRecoverMode.UntilSemicolon:
                state.SkipUntil(TokenType.Semicolon);
                if (!state.ReachedEndOfFile && state.CurrentTokenIs(TokenType.Semicolon))
                {
                    state.NextToken();
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }
}
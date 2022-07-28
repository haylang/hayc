using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HaycLib.Lexing;
using Xunit;

namespace HaycLib.Tests;

public class TokenizerTests
{
    [Fact]
    public void BasicTokenization()
    {
        const string source = @"
            function main() : int32 {
                std::String& myPrintString = ""Hello, world!"";
                std::out().writeln(myPrintString);

                std::out().writeln(""Hello, World!"");
                return 0;
            }
        ";

        Tokenizer tokenizer = new("test", source);
        IEnumerable<Token> tokens = tokenizer.Tokenize();
        Token[] tokensArr = tokens as Token[] ?? tokens.ToArray();

        tokensArr.Should().NotContain(x => x.Type == TokenType.Invalid);
        tokensArr.Should().HaveCount(41);

        /*
         * #   Type           Value
         * ------------------------------------
         * 1   Identifier     function
         * 2   Identifier     main
         * 3   LeftParen      (
         * 4   RightParen     )
         * 5   Colon          :
         * 6   Identifier     int32
         * 7   LeftBrace      {
         * 8   Identifier     std
         * 9   DoubleColon    ::
         * 10  Identifier     String
         * 11  Ampersand      &
         * 12  Identifier     myPrintString
         * 13  Assign         =
         * 14  String         "Hello, world!"
         * 15  Semicolon      ;
         * 16  Identifier     std
         * 17  DoubleColon    ::
         * 18  Identifier     out
         * 19  LeftParen      (
         * 20  RightParen     )
         * 21  Dot            .
         * 22  Identifier     writeln
         * 23  LeftParen      (
         * 24  Identifier     myPrintString
         * 25  RightParen     )
         * 26  Semicolon      ;
         * 27  Identifier     std
         * 28  DoubleColon    ::
         * 29  Identifier     out
         * 30  LeftParen      (
         * 31  RightParen     )
         * 32  Dot            .
         * 33  Identifier     writeln
         * 34  LeftParen      (
         * 35  String         "Hello, World!"
         * 36  RightParen     )
         * 37  Semicolon      ;
         * 38  Return         return
         * 39  Integer        0
         * 40  Semicolon      ;
         * 41  RightBrace     }
         */
    }

    [Fact]
    public void InvalidTokens()
    {
        const string source = "\0\u0002";

        Tokenizer tokenizer = new("test", source);
        IEnumerable<Token> tokens = tokenizer.Tokenize();
        Token[] tokensArr = tokens as Token[] ?? tokens.ToArray();

        tokensArr.Should().NotContain(x => x.Type == TokenType.Invalid);
        tokensArr.Should().HaveCount(2);
    }

    [Fact]
    public void ComplexTokens()
    {
        const string source = ":::@::=!==!=::";
        //                     AABCDDEFFGHHII = 9 tokens
        
        Tokenizer tokenizer = new("test", source);
        IEnumerable<Token> tokens = tokenizer.Tokenize();
        Token[] tokensArr = tokens as Token[] ?? tokens.ToArray();

        tokensArr.Should()
                 .Equal(
                      new[]
                      {
                          TokenType.DoubleColon,
                          TokenType.Colon,
                          TokenType.At,
                          TokenType.DoubleColon,
                          TokenType.Assign,
                          TokenType.NotEqual,
                          TokenType.Assign,
                          TokenType.NotEqual,
                          TokenType.DoubleColon
                      },
                      (a, b) => a.Type == b
                  );
    }
}